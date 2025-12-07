# Branch Protection Notes

This document outlines the branch protection rules and guidelines for the UiUxGenomeLab repository.

## Main Branch Protection

The `main` branch is protected with the following rules:

### Required Status Checks

The following CI workflows must pass before merging:

- **.NET Build and Test** - Validates code compiles and all tests pass
- **CodeQL Analysis** - Security vulnerability scanning for C# and GitHub Actions
- **Dependency Review** - Checks for vulnerable dependencies in pull requests
- **Markdown Linting** - Ensures documentation follows consistent style
- **Spell Check** - Catches spelling errors in documentation and code

### Pull Request Requirements

- **Required reviewers**: At least 1 approval from CODEOWNERS
- **Dismiss stale reviews**: Enabled - Reviews are dismissed when new commits are pushed
- **Require review from Code Owners**: Enabled
- **Restrict who can push**: Only maintainers can push directly

### Additional Protections

- **Require branches to be up to date**: PRs must be up-to-date with the base branch before merging
- **Require conversation resolution**: All PR conversations must be resolved before merging
- **Signed commits**: Recommended but not required
- **Linear history**: Prefer merge commits or squash merging to maintain clean history

## Feature Branch Guidelines

### Branch Naming Convention

Use descriptive branch names following these patterns:

- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Urgent production fixes
- `docs/description` - Documentation updates
- `refactor/description` - Code refactoring
- `test/description` - Test improvements
- `chore/description` - Maintenance tasks

### Best Practices

1. **Keep branches short-lived** - Merge within 1-2 weeks when possible
2. **Regular updates** - Sync with main frequently to minimize merge conflicts
3. **Atomic commits** - Each commit should represent a single logical change
4. **Descriptive commit messages** - Follow conventional commit format when possible
5. **Clean up** - Delete branches after merging

## Release Strategy

### Semantic Versioning

This project follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version for incompatible API changes
- **MINOR** version for backwards-compatible functionality additions
- **PATCH** version for backwards-compatible bug fixes

### Release Process

1. Create a release branch from `main`
2. Update version numbers and changelog
3. Run full test suite including E2E tests
4. Create a pull request for final review
5. Merge to `main` and tag the release
6. Deploy to production
7. Create GitHub release with notes

## Emergency Procedures

### Hotfix Process

For critical production issues:

1. Create a `hotfix/` branch from the latest release tag
2. Make minimal changes to fix the issue
3. Fast-track review with at least one maintainer approval
4. Merge to both `main` and the current release branch
5. Deploy immediately
6. Create post-mortem issue

### Reverting Changes

If a merged PR causes issues:

1. Use `git revert` to create a revert commit
2. Open a PR with the revert
3. Get expedited review from CODEOWNERS
4. Merge and deploy
5. Create an issue to properly fix the underlying problem

## Security Considerations

- All dependencies are automatically scanned for vulnerabilities
- CodeQL performs security analysis on every PR
- Security issues should be reported privately to maintainers
- Security fixes follow the hotfix process

## Enforcement

Branch protection rules are enforced automatically by GitHub. Maintainers have the ability to bypass protections in emergencies, but should document the reason.

For questions or to request changes to branch protection rules, please contact the maintainers or open an issue.
