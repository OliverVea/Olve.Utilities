from constants import REPO_ROOT
import datetime
import os

VERSION_FILE = REPO_ROOT / 'VERSION'
VERSION_FILE_DELIMITER = '.'

FORMAT_RELEASE = '{major}.{minor}.{patch}'
FORMAT_PRERELEASE = '{major}.{minor}.{patch}-{datetime}.{commit_short}'

class Version:
    """
    Version class.
    """

    def __init__(self, major: int, minor: int, patch: int):
        """
        Initialize the version.

        :param major: Major version
        :param minor: Minor version
        :param patch: Patch version
        """
        self.major = major
        self.minor = minor
        self.patch = patch

    def __str__(self):
        """
        Get the version string.

        :return: The version string
        """
        return f'{self.major}.{self.minor}.{self.patch}'

    def __repr__(self):
        """
        Get the version representation.

        :return: The version representation
        """
        return f'Version(major={self.major}, minor={self.minor}, patch={self.patch})'


def read_version() -> Version:
    """
    Read the version from the VERSION file.

    :return: The version
    """
    with open(VERSION_FILE, 'r') as f:
        version = f.read().strip()

    major, minor, patch = version.split(VERSION_FILE_DELIMITER)

    return Version(int(major), int(minor), int(patch))


def get_version_string(prerelease: bool) -> str:
    version = read_version()

    if not prerelease:
        return FORMAT_RELEASE.format(
            major=version.major,
            minor=version.minor,
            patch=version.patch)

    dt = datetime.datetime.now().strftime('%Y%m%d%H%M%S%f')
    commit_short = os.popen('git rev-parse --short HEAD').read().strip()

    return FORMAT_PRERELEASE.format(
        major=version.major,
        minor=version.minor,
        patch=version.patch,
        datetime=dt,
        commit_short=commit_short)
