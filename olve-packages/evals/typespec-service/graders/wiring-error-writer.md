---
type: regex
target: { source: file, path: src/Program.cs }
---
AddSingleton<\s*IApiErrorWriter\s*,\s*GadgetErrorWriter\s*>|AddScoped<\s*IApiErrorWriter\s*,\s*GadgetErrorWriter\s*>|AddTransient<\s*IApiErrorWriter\s*,\s*GadgetErrorWriter\s*>
