const fs = require('fs');

let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const missingCode = `
};

interface DynamicQuestion {
  id: string;
  type: 'single_choice' | 'numeric' | 'currency' | 'text' | 'yes_no' | 'taxonomy';
  label: string;
  helpText?: string;
  options?: { label: string; value: string }[];
}

const fetchDynamicQuestions = async (data: any): Promise<DynamicQuestion[]> => {
  await new Promise(resolve => setTimeout(resolve, 600));
  
  const questions: DynamicQuestion[] = [];
  
  if (data.purpose === 'Business' || data.purpose === 'Agriculture' || data.purpose === 'Skill' || data.purpose === 'Skills') {
    questions.push({
      id: 'businessActivity',
      type: 'taxonomy',
      label: 'Select Business / Economic Activity'
    });
    questions.push({
      id: 'projectCost',
      type: 'currency',
      label: 'Estimated Project Cost (₹)',
      helpText: 'Total amount needed for your business or project'
    });
  }
  
  if (data.purpose === 'Education') {
    questions.push({
      id: 'educationLevel',
      type: 'single_choice',
      label: 'Education Level',
      options: [
        { label: 'Undergraduate', value: 'Undergraduate' },
        { label: 'Postgraduate', value: 'Postgraduate' },
        { label: 'Vocational/Diploma', value: 'Vocational' }
      ]
    });
  }
  
  return questions;
};

export default function FindScheme() {
  const [step, setStep] = useState<Step>('intro');
  const [formData, setFormData] = useState<any>({
    purpose: '',
    category: '',
    dob: '',
    income: '',
    state: '',
    stateName: '',
    district: '',
    districtName: '',
    beneficiaryType: 'Myself',
    dynamicAnswers: {}
  });

  const [availableDistricts, setAvailableDistricts] = useState<{code: string, name: string}[]>([]);
  const [dynamicQuestions, setDynamicQuestions] = useState<DynamicQuestion[]>([]);
  const [isFetchingQuestions, setIsFetchingQuestions] = useState(false);
  const [isProcessing, setIsProcessing] = useState(false);
  const [schemeResults, setSchemeResults] = useState<any>(null);
  const [expandedNotEligible, setExpandedNotEligible] = useState<Record<string, boolean>>({});
  const [needsIncomeConfirmation, setNeedsIncomeConfirmation] = useState(false);
  const [isIncomeConfirmed, setIsIncomeConfirmed] = useState(false);

  const handleStateChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const name = STATES.find(s => s.code === code)?.name || '';
    setFormData({
      ...formData,
      state: code,
      stateName: name,
      district: '',
      districtName: '',
      dynamicAnswers: {}
    });
    const districts = await fetchDistrictsFromBackend(code);
    setAvailableDistricts(districts);
  };

  const handleDistrictChange =`;

content = content.replace(/};\s*const handleDistrictChange =/, missingCode);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
