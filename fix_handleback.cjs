const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const regexHandleBack = /const handleBack = \(\) => \{[\s\S]*?return newHistory;\n\s*\}\);\n\s*\};/;
const newHandleBack = `const handleBack = () => {
    if (step === 'purpose') {
      navigate(-1);
      return;
    }
    setStepHistory(prev => {
      const newHistory = [...prev];
      const previousStep = newHistory.pop();
      if (previousStep) {
        setStep(previousStep);
        window.scrollTo(0, 0);
      }
      return newHistory;
    });
  };`;

content = content.replace(regexHandleBack, newHandleBack);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
