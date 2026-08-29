const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

// Replace setFormData({...formData, ...}) safely
content = content.replace(/setFormData\(\{\s*\.\.\.formData,\s*([^}]+)\}\)/g, 'setFormData(prev => ({ ...prev, $1 }))');

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
