const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

content = content.replace(
  /onClick=\{\(\) => \{\n\s*setFormData\(\{ \.\.\.formData, purpose: purpose\.id \}\);\n\s*handleNext\('about'\);\n\s*\}\}/,
  `onClick={() => {
              if (formData.purpose !== purpose.id) {
                setFormData({ ...formData, purpose: purpose.id, dynamicAnswers: {} });
              } else {
                setFormData({ ...formData, purpose: purpose.id });
              }
              handleNext('about');
            }}`
);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
