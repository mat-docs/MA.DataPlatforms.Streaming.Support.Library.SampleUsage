# Core Library Documentation Sync

This document explains how to sync documentation from the core C# library into this sample usage repository.

## Overview

The `docs/core-library/` folder contains documentation from the **MA DataPlatforms Streaming Support Library** core C# library. This documentation should be synced from the main library repository to provide comprehensive API and library reference alongside the sample code documentation.

## Quick Start

### 1. Prerequisites

Ensure the core library repository is cloned alongside this repository:

```
repos/
├── MA.DataPlatforms.Streaming.Support.Library/
│   └── docs/
└── MA.DataPlatforms.Streaming.Support.Library.SampleUsage/
    └── (this repository)
```

### 2. Sync Documentation

Copy all files from the core library docs folder to `docs/core-library/`:

**Windows (PowerShell):**
```powershell
# From repository root
Copy-Item -Path "..\MA.DataPlatforms.Streaming.Support.Library\docs\*" -Destination "docs\core-library\" -Recurse -Force
```

**Windows (Command Prompt):**
```cmd
xcopy /E /Y /I ..\MA.DataPlatforms.Streaming.Support.Library\docs\* docs\core-library\
```

**Linux/Mac:**
```bash
cp -r ../MA.DataPlatforms.Streaming.Support.Library/docs/* docs/core-library/
```

### 3. Verify

Check that the documentation was synced:

```powershell
# List synced files
Get-ChildItem docs\core-library -Recurse

# Test the documentation site
mkdocs serve
```

## Advanced Usage

### Custom Source Path

If the core library is located elsewhere, adjust the path in your copy command:

```powershell
Copy-Item -Path "C:\Projects\CoreLibrary\docs\*" -Destination "docs\core-library\" -Recurse -Force
```

### Partial Sync

To sync only specific folders:

```powershell
# Example: sync only API docs
Copy-Item -Path "..\MA.DataPlatforms.Streaming.Support.Library\docs\api\*" -Destination "docs\core-library\api\" -Recurse -Force
```

## Workflow Integration

### Manual Workflow

1. Update core library documentation in the main repository
2. Copy files manually or via your pipeline
3. Commit changes to `docs/core-library/` (if tracking in git)
4. Build and deploy documentation

### Automated Workflow (CI/CD Pipeline)

#### GitHub Actions Example

```yaml
name: Sync Core Docs

on:
  workflow_dispatch:  # Manual trigger
  schedule:
    - cron: '0 0 * * 0'  # Weekly

jobs:
  sync-docs:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout this repo
        uses: actions/checkout@v3
        
      - name: Checkout core library
        uses: actions/checkout@v3
        with:
          repository: org/MA.DataPlatforms.Streaming.Support.Library
          path: core-lib
          
      - name: Copy documentation
        run: |
          cp -r core-lib/docs/* docs/core-library/
          
      - name: Commit changes
        run: |
          git config user.name "GitHub Actions"
          git config user.email "actions@github.com"
          git add docs/core-library/
          git commit -m "Sync core library documentation" || echo "No changes"
          git push
```

#### Azure DevOps Example

```yaml
trigger: none  # Manual only

steps:
- checkout: self
  
- checkout: git://Project/MA.DataPlatforms.Streaming.Support.Library
  path: core-lib

- task: PowerShell@2
  displayName: 'Sync Documentation'
  inputs:
    targetType: 'inline'
    script: |
      Copy-Item -Path "$(Pipeline.Workspace)/core-lib/docs/*" -Destination "$(Build.SourcesDirectory)/docs/core-library/" -Recurse -Force
      
- task: PowerShell@2
  displayName: 'Commit Changes'
  inputs:
    targetType: 'inline'
    script: |
      git config user.name "Azure Pipelines"
      git config user.email "pipelines@azure.com"
      git add docs/core-library/
      git commit -m "Sync core library documentation" || echo "No changes"
      git push
```

## Git Configuration

### Option 1: Track Synced Docs (Recommended)

Commit the synced documentation to version control:

```powershell
git add docs/core-library/
git commit -m "Sync core library documentation"
```

**Pros:**
- Documentation is always available
- No dependency on external repository
- Works offline

**Cons:**
- Larger repository size
- Need to sync and commit when core docs change

### Option 2: Ignore Synced Docs

Add to `.gitignore`:

```gitignore
docs/core-library/*
!docs/core-library/README.md
```

**Pros:**
- Smaller repository
- Always get latest docs from source

**Cons:**
- Must sync before building docs
- Requires access to core repository

## MkDocs Integration

The synced documentation is automatically included in the MkDocs navigation via `mkdocs.yml`:

```yaml
nav:
  - Core Library Docs: core-library/
```

MkDocs will automatically discover and index all markdown files in the `docs/core-library/` folder.

### Custom Navigation

To customize the core library docs navigation:

```yaml
nav:
  - Core Library:
    - Overview: core-library/index.md
    - API Reference: core-library/api/
    - Guides: core-library/guides/
```

## Troubleshooting

### Source Path Not Found

**Error:** Cannot find source documentation folder

**Solution:**
- Verify the core library is cloned in the expected location
- Check the path in your copy command
- Ensure the `docs` folder exists in the core library

### Permission Errors

**Error:** `Access denied` or permission issues

**Solution:**
- Run command prompt/PowerShell as Administrator
- Check file/folder permissions
- Close any applications that might have files open

### Empty Sync

**Issue:** Command runs but no files are copied

**Solution:**
- Verify source folder contains markdown files
- Check if source path is correct  
- Look for hidden files or access restrictions
- Use `-Verbose` flag to see what's happening

## File Structure

After syncing, your structure will look like:

```
docs/
├── core-library/          # Synced from core library
│   ├── README.md          # Sync instructions (this file)
│   ├── index.md           # Core library docs home (if exists)
│   ├── api/               # API documentation
│   ├── guides/            # User guides
│   └── ...                # Other core library docs
├── getting-started/       # Sample usage docs
├── python/
├── csharp/
└── ...
```

## Best Practices

1. **Regular Syncs**: Sync documentation regularly to keep it up-to-date
2. **Version Alignment**: Ensure core docs version matches library version used in samples
3. **Review Changes**: After syncing, review changes before committing
4. **Test Locally**: Always test with `mkdocs serve` before deploying
5. **Document Versions**: Note which core library version the docs are synced from
6. **Automate**: Use CI/CD pipelines for consistent syncing

## Support

For issues with:
- **Copying/syncing**: Check file permissions and paths
- **Core library docs**: Contact core library documentation team
- **MkDocs integration**: See [MKDOCS_SETUP.md](MKDOCS_SETUP.md)

---

Last updated: 2025-R03
