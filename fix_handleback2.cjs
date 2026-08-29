const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const regexHandleBack = /const handleBack = \(\) => \{\n\s*if \(step === 'purpose'\) \{\n\s*navigate\(-1\);\n\s*return;\n\s*\}/;

const newHandleBack = `const handleBack = () => {
    if (step === 'purpose') {
      if (window.history.state && window.history.state.idx > 0) {
        navigate(-1);
      } else {
        navigate('/');
      }
      return;
    }`;

content = content.replace(regexHandleBack, newHandleBack);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
