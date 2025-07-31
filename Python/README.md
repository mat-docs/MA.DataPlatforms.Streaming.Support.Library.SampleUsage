# Python Streaming Support Library Sample Code
The following repo contains sample code for use with the Python Streaming Support Library. There are two examples given:

## Sample Reader
This is an example on how to read live and historic sessions from the broker. This example will read the session and
give you the value of `vCar:Chassis` along with any updates to the session info, current coverage cursor value, and 
which streams it is reading from.

## Sample Writer
This is an example on how to write sessions into the broker. This example will create a session on the broker and setup
one parameter called `Sin:MyApp` which is just a sine wave. After all the samples are written, the session will stop.
This session can be recorded using the Stream Recorder on ATLAS.

## Setup
Before starting, it is recommended to use a Python venv for development. Then use the following command to get the
required dependencies:

`python -m pip install -r Requirements.txt`

Once done, then you can go to the main of each example and run them individually.