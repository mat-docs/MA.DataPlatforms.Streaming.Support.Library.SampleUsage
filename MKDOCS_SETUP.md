# MkDocs Documentation Setup

This repository uses [MkDocs](https://www.mkdocs.org/) with the [Material theme](https://squidfunk.github.io/mkdocs-material/) for documentation.

## Prerequisites

- **Python 3.8+**
- **pip** (Python package installer)

## Quick Setup

### 1. Install MkDocs and Dependencies

```bash
# Install from requirements file
pip install -r docs-requirements.txt
```

Or install manually:

```bash
pip install mkdocs mkdocs-material mkdocs-minify-plugin pymdown-extensions
```

### 2. Serve Documentation Locally

```bash
# Run the development server
mkdocs serve
```

The documentation will be available at: **http://127.0.0.1:8000**

The server auto-reloads when you make changes to the documentation files.

### 3. Build Static Site

```bash
# Build the documentation site
mkdocs build
```

This creates a `site/` directory with static HTML files.

## Project Structure

```
.
├── mkdocs.yml                  # MkDocs configuration
├── docs/                       # Documentation source files
│   ├── index.md               # Home page
│   ├── getting-started/       # Getting started guides
│   ├── python/                # Python documentation
│   ├── csharp/                # C# documentation
│   ├── advanced/              # Advanced topics
│   ├── reference/             # Reference documentation
│   ├── stylesheets/           # Custom CSS
│   └── javascripts/           # Custom JavaScript
├── docs-requirements.txt      # Python dependencies for docs
└── site/                      # Generated site (git-ignored)
```

## Common Commands

| Command | Description |
|---------|-------------|
| `mkdocs serve` | Start development server at http://127.0.0.1:8000 |
| `mkdocs serve -a 0.0.0.0:8000` | Serve on all network interfaces |
| `mkdocs build` | Build static site to `site/` directory |
| `mkdocs build --clean` | Build and remove old files |
| `mkdocs gh-deploy` | Deploy to GitHub Pages |

## Writing Documentation

### File Format

- All documentation files are in **Markdown** format (`.md`)
- Use [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/reference/) features
- Supports Python Markdown extensions

### Code Blocks

Use fenced code blocks with language specification:

````markdown
```python
# Python code example
print("Hello, World!")
```

```csharp
// C# code example
Console.WriteLine("Hello, World!");
```
````

### Admonitions

Create notes, warnings, tips:

```markdown
!!! note "Custom Title"
    This is a note admonition.

!!! warning
    This is a warning!

!!! tip
    This is a helpful tip!
```

### Tabs

Create tabbed content:

```markdown
=== "Python"
    Python content here

=== "C#"
    C# content here
```

### Diagrams

Use SVG images for diagrams (stored in `docs/images/`):

```markdown
![Diagram Description](../images/your-diagram.svg)
```

SVG images provide better compatibility and don't require plugins.

## Customization

### Theme Colors

Edit `mkdocs.yml` to change theme colors:

```yaml
theme:
  palette:
    primary: indigo  # Change primary color
    accent: indigo   # Change accent color
```

Available colors: red, pink, purple, deep purple, indigo, blue, light blue, cyan, teal, green, light green, lime, yellow, amber, orange, deep orange

### Custom CSS

Add custom styles to `docs/stylesheets/extra.css`

### Custom JavaScript

Add custom scripts to `docs/javascripts/`

## Deployment

### GitHub Pages

```bash
# Deploy to GitHub Pages (creates/updates gh-pages branch)
mkdocs gh-deploy
```

### Manual Deployment

1. Build the site: `mkdocs build`
2. Copy contents of `site/` directory to your web server
3. Configure web server to serve static HTML files

## Troubleshooting

### Port Already in Use

```bash
# Use a different port
mkdocs serve -a 127.0.0.1:8001
```

### Build Errors

```bash
# Check configuration
mkdocs build --verbose

# Validate configuration
python -c "import yaml; yaml.safe_load(open('mkdocs.yml'))"
```

### Missing Dependencies

```bash
# Reinstall all dependencies
pip install -r docs-requirements.txt --upgrade
```

## Additional Resources

- [MkDocs Documentation](https://www.mkdocs.org/)
- [Material for MkDocs](https://squidfunk.github.io/mkdocs-material/)
- [Python Markdown Extensions](https://facelessuser.github.io/pymdown-extensions/)
- [Mermaid Diagrams](https://mermaid-js.github.io/)

## Support

For documentation-related issues:
1. Check MkDocs documentation
2. Refer to Material for MkDocs reference
3. Contact Motion Applied support for content questions
