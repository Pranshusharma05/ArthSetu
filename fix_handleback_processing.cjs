const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const target = `  const handleBack = () => {
    if (step === 'purpose') {
      if (window.history.state && window.history.state.idx > 0) {
        navigate(-1);
      } else {
        navigate('/');
      }
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

const replacement = `  const handleBack = () => {
    if (step === 'purpose') {
      if (window.history.state && window.history.state.idx > 0) {
        navigate(-1);
      } else {
        navigate('/');
      }
      return;
    }
    setStepHistory(prev => {
      let newHistory = [...prev];
      let previousStep = newHistory.pop();
      while (previousStep === 'processing' && newHistory.length > 0) {
        previousStep = newHistory.pop();
      }
      if (previousStep) {
        setStep(previousStep);
        window.scrollTo(0, 0);
      }
      return newHistory;
    });
  };`;

if(content.includes(target)) {
  content = content.replace(target, replacement);
  fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
} else {
  console.log("Could not find handleBack function to replace.");
}
