---
type: regex
target: { source: file, path: src/RenameGadgetHandler.cs }
weight: 2
---
class\s+\w+\s*(\([^)]*\)\s*)?:\s*IGadgetsRenameHandler\b
