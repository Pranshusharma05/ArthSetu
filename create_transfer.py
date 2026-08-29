import tarfile
import os
import fnmatch

output_filename = "ArthSetu-transfer.tar.gz"
exclude_dirs = {"node_modules", "bin", "obj", ".git", "dist", ".vite"}
exclude_patterns = ["*.env*", "*.sqlite", "*.db", "*.suo", "*.user", output_filename, "create_transfer.py", "create_zip.py", "ArthSetu-AIStudio-Complete.zip"]

def should_exclude_dir(dirpath):
    parts = dirpath.split(os.sep)
    for d in exclude_dirs:
        if d in parts:
            return True
    return False

def should_exclude_file(filename):
    for pat in exclude_patterns:
        if fnmatch.fnmatch(filename, pat):
            return True
    return False

with tarfile.open(output_filename, "w:gz") as tar:
    for root, dirs, files in os.walk("."):
        # Modify dirs in place to prevent traversing excluded dirs
        dirs[:] = [d for d in dirs if d not in exclude_dirs]
        
        for file in files:
            if not should_exclude_file(file):
                filepath = os.path.join(root, file)
                if not should_exclude_dir(filepath):
                    arcname = os.path.relpath(filepath, ".")
                    tar.add(filepath, arcname=arcname)

print(f"Created {output_filename}")
