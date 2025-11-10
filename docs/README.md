# Documentation Source

This directory contains the source files for the MkDocs documentation site.

## Structure

- `index.md` - Home page
- `getting-started/` - Installation, overview, quick start
- `python/` - Python implementation guide
- `csharp/` - C# implementation guide  
- `advanced/` - Advanced topics (architecture, performance, migration)
- `reference/` - Reference documentation (configuration, feature comparison, glossary)
- `support.md` - Support and help resources
- `stylesheets/` - Custom CSS
- `javascripts/` - Custom JavaScript

## Building Documentation

See the [MKDOCS_SETUP.md](../MKDOCS_SETUP.md) file in the root directory for complete setup instructions.

### Quick Start

```powershell
# From repository root
pip install -r docs-requirements.txt
mkdocs serve
```

Then open http://127.0.0.1:8000 in your browser.

### Or Use the Helper Script

```powershell
.\serve-docs.ps1
```

## Writing Documentation

All files are in Markdown format with Material for MkDocs extensions enabled. See [Material for MkDocs Reference](https://squidfunk.github.io/mkdocs-material/reference/) for available features.

### Supported Features

- Admonitions (notes, warnings, tips)
- Code blocks with syntax highlighting
- Tabs for multi-language content
- Mermaid diagrams
- Material icons
- Grid cards
- And much more...

## Adding New Pages

1. Create a new `.md` file in the appropriate directory
2. Add the page to `nav` section in `mkdocs.yml`
3. Link to the page from other relevant pages

Example `mkdocs.yml` entry:

```yaml
nav:
  - Python Guide:
    - New Page: python/new-page.md
```
