const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

// 1. Add useNavigate to imports
content = content.replace(
  "import { Link } from 'react-router-dom';",
  "import { Link, useNavigate } from 'react-router-dom';"
);

// 2. Add navigate inside FindScheme component
content = content.replace(
  "export function FindScheme() {",
  "export function FindScheme() {\n  const navigate = useNavigate();"
);

// 3. Update handleBack logic
const oldHandleBack = `  const handleBack = () => {
    setStepHistory(prev => {
      const newHistory = [...prev];
      const previousStep = newHistory.pop();
      if (previousStep) {
        setStep(previousStep);
      }
      return newHistory;
    });
    window.scrollTo(0, 0);
  };`;

const newHandleBack = `  const handleBack = () => {
    if (step === 'purpose') {
      navigate(-1);
      return;
    }
    setStepHistory(prev => {
      const newHistory = [...prev];
      const previousStep = newHistory.pop();
      if (previousStep) {
        setStep(previousStep);
      }
      return newHistory;
    });
    window.scrollTo(0, 0);
  };`;

if(content.includes(oldHandleBack)) {
  content = content.replace(oldHandleBack, newHandleBack);
} else {
  console.log("Could not find old handleBack");
}

// 4. Show the back button on 'purpose' step as well, but not on 'intro', 'processing', 'result'
// Current rendering logic is around line 1300:
/*
      {step !== 'intro' && step !== 'processing' && step !== 'result' && (
        <div className="max-w-2xl mx-auto px-4 mb-12">
          {step !== 'purpose' && (
            <button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-4 text-[14px]">
              <ArrowLeft className="w-4 h-4" />
              Back
            </button>
          )}
*/
const oldBackRender = `{step !== 'purpose' && (
            <button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-4 text-[14px]">
              <ArrowLeft className="w-4 h-4" />
              Back
            </button>
          )}`;

const newBackRender = `<button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-4 text-[14px]">
              <ArrowLeft className="w-4 h-4" />
              Back
            </button>`;

if(content.includes(oldBackRender)) {
  content = content.replace(oldBackRender, newBackRender);
} else {
  console.log("Could not find old back button render");
}

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
