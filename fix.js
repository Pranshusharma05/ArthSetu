const fs = require('fs');
let adminCode = fs.readFileSync('src/pages/Admin.tsx', 'utf8');
adminCode = adminCode.replace(/fetch\(\\\$\{import.meta.env.VITE_API_BASE_URL \|\| 'http:\/\/localhost:5000'\}\/api\/admin\/sources\)/g, 'fetch($/api/admin/sources)');
fs.writeFileSync('src/pages/Admin.tsx', adminCode);

let partnerCode = fs.readFileSync('src/pages/FindPartner.tsx', 'utf8');
partnerCode = partnerCode.replace(/const url = schemeId \? \\\\\\$\{baseUrl\}\/api\/partners\?schemeId=\\\$\{schemeId\}\\\ : \\\\\\$\{baseUrl\}\/api\/partners\\\;/g, 'const url = schemeId ? $/api/partners?schemeId={schemeId} : $/api/partners;');
fs.writeFileSync('src/pages/FindPartner.tsx', partnerCode);
