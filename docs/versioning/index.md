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

## Commitzen

### Commands

**`cz version -p`:** Returns the current version number.


## Process

1. Work on `develop` branch
    - Todo: Nightly builds from develop
2. Commit changes with `cz commit`
3. Create PR to `master` branch
4. CI will run to ensure that the code is working
5. Merge PR
6. [commitizen-tools/commitizen-action](https://github.com/commitizen-tools/commitizen-action) bumps
    - The version number
    - Creates a tag, e.g. `v1.2.3`
    - Creates a changelog
    - Pushes the changes to the `master` branch
7. Code is deployed to production


[bump version](https://github.com/commitizen-tools/commitizen-action)
[create release](https://github.com/softprops/action-gh-release)
[add files to release](https://github.com/marketplace/actions/upload-files-to-a-github-release)