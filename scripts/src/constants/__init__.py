import pathlib
import git

__repo_root = git.Repo('.', search_parent_directories=True)
REPO_ROOT = pathlib.Path(__repo_root.working_tree_dir)