// <copyright file="SqlSessionManager.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using MA.DataPlatforms.Streaming.Support.Lib.Core.Shared.Abstractions;

using MAT.OCS.Core;

using MESL.SqlRace.Domain;
using MESL.SqlRace.Enumerators;

namespace MA.DataPlatforms.Streaming.Support.Library.SampleUsage.Buffering.Interpolation.SqlRace;

internal class SqlSessionManager : ISqlSessionManager
{
    private readonly Dictionary<string, SqlRaceSession> sessionDictionary = new Dictionary<string, SqlRaceSession>();
    private readonly SessionManager sessionManager;
    private readonly ConfigurationSetManager configurationSetManager;
    private readonly string connectionString;
    private readonly object sessionDictionaryLock = new object();
    private readonly List<string> subscribedParameters;
    private readonly ILogger logger;

    public SqlSessionManager(string connectionString, List<string> subscribedParameters, ILogger logger)
    {
        this.sessionManager = SessionManager.CreateSessionManager();
        this.configurationSetManager = new ConfigurationSetManager();
        this.connectionString = connectionString;
        this.subscribedParameters = subscribedParameters;
        this.logger = logger;
    }

    public SqlRaceSession CreateSession(string sessionKey)
    {
        lock (this.sessionDictionaryLock)
        {
            if (this.sessionDictionary.TryGetValue(sessionKey, out var session))
            {
                return session;
            }

            this.logger.Info("Creating Session");
            var sqlSession = this.sessionManager.CreateSession(this.connectionString, SessionKey.NewKey(), "SupportLibDemo", DateTime.Now, "Session");
            var config = this.configurationSetManager.Create(sqlSession.Session.Key, this.connectionString, Guid.NewGuid().ToString(), "");
            var conversion = RationalConversion.CreateSimple1To1Conversion("DefaultConversion", "", "%5.2f");
            config.AddConversion(conversion);
            var channels = new List<Channel>();
            var parameters = new List<Parameter>();
            var channelDictionary = new Dictionary<string, uint>();
            var channelId = 0U;

            var applicationGroup = new ApplicationGroup("SupportLibraryDemo", "Support Library Demo");
            config.AddGroup(applicationGroup);

            foreach (var parameterIdentifier in this.subscribedParameters)
            {
                for (var i = 0; i < 8; i++)
                {
                    var parameterName = parameterIdentifier.Split(':')[0];
                    switch (i)
                    {
                        case 0:
                        {
                            parameterName += ParameterConstants.BufferedParameter;
                            break;
                        }
                        case 1:
                        {
                            parameterName += ParameterConstants.BufferedTimestamp;
                            break;
                        }
                        case 2:
                        {
                            parameterName += ParameterConstants.InterpolatedMin;
                            break;
                        }
                        case 3:
                        {
                            parameterName += ParameterConstants.InterpolatedMax;
                            break;
                        }
                        case 4:
                        {
                            parameterName += ParameterConstants.InterpolatedFirst;
                            break;
                        }
                        case 5:
                        {
                            parameterName += ParameterConstants.InterpolatedLast;
                            break;
                        }
                        case 6:
                        {
                            parameterName += ParameterConstants.InterpolatedMean;
                            break;
                        }
                        case 7:
                        {
                            parameterName += ParameterConstants.LinearInterpolation;
                            break;
                        }
                    }

                    var channel = new Channel(channelId, parameterName, 0, DataType.Double64Bit, ChannelDataSourceType.RowData);
                    channels.Add(channel);
                    channelDictionary[parameterName] = channelId;
                    var parameter = new Parameter(
                        $"{parameterName}:{applicationGroup.Name}",
                        parameterName,
                        "",
                        100,
                        0,
                        100,
                        0,
                        0,
                        0xFFFFFFFF,
                        0,
                        "DefaultConversion",
                        new List<string>(),
                        new List<uint>
                        {
                            channelId
                        },
                        applicationGroup.Name,
                        "%5.2f");
                    parameters.Add(parameter);
                    channelId++;
                }
            }

            config.AddChannels(channels);
            config.AddParameters(parameters);

            this.logger.Info("Committing Config");
            config.Commit();
            sqlSession.Session.UseLoggingConfigurationSet(config.Identifier);
            this.logger.Info("Config Committed");

            session = new SqlRaceSession(sqlSession, channelDictionary);
            this.sessionDictionary[sessionKey] = session;

            return session;
        }
    }

    public SqlRaceSession? GetSession(string sessionKey)
    {
        lock (this.sessionDictionaryLock)
        {
            return this.sessionDictionary.GetValueOrDefault(sessionKey);
        }
    }

    public void Stop()
    {
        lock (this.sessionDictionaryLock)
        {
            foreach (var session in this.sessionDictionary)
            {
                if (session.Value.ClientSession.Session.State == SessionState.Live)
                {
                    EndSession(session.Value);
                }

                session.Value.ClientSession.Close();
            }
        }
    }

    public void StopSession(string sessionKey)
    {
        lock (this.sessionDictionaryLock)
        {
            if (!this.sessionDictionary.TryGetValue(sessionKey, out var session))
            {
                return;
            }

            if (session.ClientSession.Session.State == SessionState.Historical)
            {
                return;
            }

            EndSession(session);
        }
    }

    private static void EndSession(SqlRaceSession session)
    {
        session.ClientSession.Session.EndData();
        session.ClientSession.Session.Flush();
    }
}