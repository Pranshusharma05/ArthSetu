const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

content = content.replace(/scheme\.evaluation/g, 'scheme.ruleComparisons');
content = content.replace(/evalItem\.name/g, 'evalItem.ruleName');

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
