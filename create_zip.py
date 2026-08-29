import zipfile
import os
import fnmatch

output_filename = "ArthSetu-AIStudio-Complete.zip"
exclude_dirs = {"node_modules", "bin", "obj", ".git", "dist"}
exclude_patterns = ["*.env", "*.sqlite", "*.db", "*.suo", "*.user", output_filename, "create_zip.py"]

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

with zipfile.ZipFile(output_filename, 'w', zipfile.ZIP_DEFLATED) as zipf:
    for root, dirs, files in os.walk("."):
        # Modify dirs in place to prevent os.walk from traversing excluded dirs
        dirs[:] = [d for d in dirs if d not in exclude_dirs]
        
        for file in files:
            if not should_exclude_file(file):
                filepath = os.path.join(root, file)
                if not should_exclude_dir(filepath):
                    arcname = os.path.relpath(filepath, ".")
                    zipf.write(filepath, arcname)

print(f"Created {output_filename}")
