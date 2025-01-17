# Versioning

**links:**

- [semver](https://semver.org/)
- [conventional commits](https://www.conventionalcommits.org/en/about/)

## Tools

### Local tools

We need a git hook to enforce the commit message format.

- [commitizen](https://commitizen-tools.github.io/commitizen/)
  - Features:
    - CLI tool for conventional commits
    - Automatically generates changelog
    - Automatically increments version from conventional commits
    - Automatically generates version tags
  - Required:
    - Python 3.9+

### GitHub Actions

- [changelog from conventional commits](https://github.com/marketplace/actions/changelog-from-conventional-commits)
- [semver from conventional commits](https://github.com/ietf-tools/semver-action)
