import React, { useEffect, useState } from 'react';
import {
  ArrowLeft,
  ArrowRight,
  AlertCircle,
  Baby,
  Briefcase,
  Bus,
  ChevronDown,
  ChevronUp,
  Droplet,
  GraduationCap,
  Heart,
  Home,
  Info,
  Landmark,
  Map,
  MapPin,
  Monitor,
  Search,
  Shield,
  ShieldCheck,
  Trophy,
  Users,
  Wrench,
  XCircle,
} from 'lucide-react';

type Step = 'intro' | 'purpose' | 'about' | 'financial' | 'location' | 'specifics' | 'review' | 'processing' | 'result';

const PURPOSES = [
  { id: 'Agriculture', icon: Map, title: 'Agriculture, Rural & Environment', desc: 'Farming, dairy, fisheries and rural livelihood support' },
  { id: 'Banking', icon: Landmark, title: 'Banking & Financial Support', desc: 'Credit, insurance and financial inclusion schemes' },
  { id: 'Business', icon: Briefcase, title: 'Business & Entrepreneurship', desc: 'Startup, self-employment, services or manufacturing' },
  { id: 'Education', icon: GraduationCap, title: 'Education & Learning', desc: 'Scholarships, education loans and student support' },
  { id: 'Health', icon: Heart, title: 'Health & Wellness', desc: 'Healthcare and medical assistance' },
  { id: 'Housing', icon: Home, title: 'Housing & Shelter', desc: 'Housing, home construction and improvement support' },
  { id: 'PublicSafety', icon: Shield, title: 'Public Safety, Law & Justice', desc: 'Eligible legal and public-safety support programmes' },
  { id: 'Science', icon: Monitor, title: 'Science, IT & Communications', desc: 'Technology, digital and science-related support' },
  { id: 'Skills', icon: Wrench, title: 'Skills & Employment', desc: 'Training, employment and livelihood support' },
  { id: 'SocialWelfare', icon: Users, title: 'Social Welfare & Empowerment', desc: 'Benefits for eligible social and welfare groups' },
  { id: 'Sports', icon: Trophy, title: 'Sports & Culture', desc: 'Eligible sports and cultural support schemes' },
  { id: 'Transport', icon: Bus, title: 'Transport & Mobility', desc: 'Eligible transport and mobility support' },
  { id: 'Travel', icon: MapPin, title: 'Travel & Tourism', desc: 'Tourism and hospitality support programmes' },
  { id: 'Utility', icon: Droplet, title: 'Utility & Sanitation', desc: 'Water, sanitation and utility support' },
  { id: 'WomenChild', icon: Baby, title: 'Women & Child', desc: 'Schemes specifically supporting women and children' },
];

const ACTIVITY_CATEGORIES = [
  { id: 'Agriculture', title: 'Agriculture' }, { id: 'Dairy', title: 'Dairy / Livestock' },
  { id: 'Poultry', title: 'Poultry' }, { id: 'Fisheries', title: 'Fisheries' },
  { id: 'Food Processing', title: 'Food Processing' }, { id: 'Handloom', title: 'Handloom' },
  { id: 'Handicrafts', title: 'Handicrafts' }, { id: 'Textiles', title: 'Textiles / Garments' },
  { id: 'Manufacturing', title: 'Manufacturing' }, { id: 'Trading', title: 'Trading / Retail' },
  { id: 'Professional', title: 'Professional Services' }, { id: 'Repair', title: 'Repair & Maintenance' },
  { id: 'Transport', title: 'Transport / Logistics' }, { id: 'Tourism', title: 'Tourism / Hospitality' },
  { id: 'Digital', title: 'Digital / IT' }, { id: 'Construction', title: 'Construction' },
  { id: 'SelfEmployment', title: 'Self-Employment' }, { id: 'SHG', title: 'SHG / Group Enterprise' },
  { id: 'Startup', title: 'Startup' }, { id: 'Other', title: 'Other' },
];

const STATES = [
  { code: 'AN', name: 'Andaman and Nicobar Islands' }, { code: 'AP', name: 'Andhra Pradesh' },
  { code: 'AR', name: 'Arunachal Pradesh' }, { code: 'AS', name: 'Assam' },
  { code: 'BR', name: 'Bihar' }, { code: 'CH', name: 'Chandigarh' },
  { code: 'CT', name: 'Chhattisgarh' }, { code: 'DN', name: 'Dadra and Nagar Haveli and Daman and Diu' },
  { code: 'DL', name: 'Delhi' }, { code: 'GA', name: 'Goa' },
  { code: 'GJ', name: 'Gujarat' }, { code: 'HR', name: 'Haryana' },
  { code: 'HP', name: 'Himachal Pradesh' }, { code: 'JK', name: 'Jammu and Kashmir' },
  { code: 'JH', name: 'Jharkhand' }, { code: 'KA', name: 'Karnataka' },
  { code: 'KL', name: 'Kerala' }, { code: 'LA', name: 'Ladakh' },
  { code: 'LD', name: 'Lakshadweep' }, { code: 'MP', name: 'Madhya Pradesh' },
  { code: 'MH', name: 'Maharashtra' }, { code: 'MN', name: 'Manipur' },
  { code: 'ML', name: 'Meghalaya' }, { code: 'MZ', name: 'Mizoram' },
  { code: 'NL', name: 'Nagaland' }, { code: 'OR', name: 'Odisha' },
  { code: 'PY', name: 'Puducherry' }, { code: 'PB', name: 'Punjab' },
  { code: 'RJ', name: 'Rajasthan' }, { code: 'SK', name: 'Sikkim' },
  { code: 'TN', name: 'Tamil Nadu' }, { code: 'TG', name: 'Telangana' },
  { code: 'TR', name: 'Tripura' }, { code: 'UP', name: 'Uttar Pradesh' },
  { code: 'UT', name: 'Uttarakhand' }, { code: 'WB', name: 'West Bengal' },
];

interface DynamicQuestion {
  id: string;
  type: 'single_choice' | 'numeric' | 'currency' | 'text' | 'yes_no' | 'taxonomy';
  label: string;
  helpText?: string;
  options?: { label: string; value: string }[];
}

const apiBase = () => import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';

const fetchDistrictsFromBackend = async (stateCode: string): Promise<{ code: string; name: string }[]> => {
  const res = await fetch(`${apiBase()}/api/locations/districts?state=${encodeURIComponent(stateCode)}`);
  if (!res.ok) throw new Error('District data could not be loaded from the verified location source.');
  return await res.json();
};

const fetchDynamicQuestions = async (data: any): Promise<DynamicQuestion[]> => {
  const res = await fetch(`${apiBase()}/api/schemes/dynamic-questions`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!res.ok) throw new Error('Additional eligibility questions could not be loaded.');
  return await res.json();
};

const TaxonomySelector = ({ value, onChange }: { value: any; onChange: (val: any) => void }) => (
  <div className="space-y-4">
    <select
      value={value.activity || ''}
      onChange={(e) => onChange({ ...value, activity: e.target.value })}
      className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-semibold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
    >
      <option value="">Select an activity</option>
      {ACTIVITY_CATEGORIES.map((c) => <option key={c.id} value={c.id}>{c.title}</option>)}
    </select>
    {value.activity === 'Other' && (
      <input
        type="text"
        placeholder="Please specify"
        value={value.customActivityText || ''}
        onChange={(e) => onChange({ ...value, customActivityText: e.target.value })}
        className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-semibold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
      />
    )}
  </div>
);

export function FindScheme() {
  const [step, setStep] = useState<Step>('intro');
  const [formData, setFormData] = useState<any>({
    purpose: '', category: '', dob: '', income: '', state: '', stateName: '', district: '', districtName: '',
    beneficiaryType: 'Myself', dynamicAnswers: {},
  });
  const [availableDistricts, setAvailableDistricts] = useState<{ code: string; name: string }[]>([]);
  const [dynamicQuestions, setDynamicQuestions] = useState<DynamicQuestion[]>([]);
  const [isFetchingQuestions, setIsFetchingQuestions] = useState(false);
  const [isLoadingDistricts, setIsLoadingDistricts] = useState(false);
  const [locationError, setLocationError] = useState('');
  const [schemeResults, setSchemeResults] = useState<any>(null);
  const [schemeError, setSchemeError] = useState('');
  const [expandedDetails, setExpandedDetails] = useState<Record<string, boolean>>({});
  const [expandedNotEligible, setExpandedNotEligible] = useState<Record<string, boolean>>({});
  const [expandedMoreInfo, setExpandedMoreInfo] = useState(false);
  const [expandedAlternative, setExpandedAlternative] = useState(false);
  const [showOptionalAttributes, setShowOptionalAttributes] = useState(false);

  useEffect(() => {
    const state = window.history.state;
    if (!state?.arthSetuStep) {
      window.history.replaceState({ ...(state || {}), arthSetuStep: 'intro' }, '');
    }
    const onPopState = (event: PopStateEvent) => {
      const previousStep = event.state?.arthSetuStep as Step | undefined;
      if (previousStep) {
        setStep(previousStep);
        window.scrollTo(0, 0);
      }
    };
    window.addEventListener('popstate', onPopState);
    return () => window.removeEventListener('popstate', onPopState);
  }, []);

  const goToStep = (nextStep: Step, push = true) => {
    if (push) window.history.pushState({ ...(window.history.state || {}), arthSetuStep: nextStep }, '');
    else window.history.replaceState({ ...(window.history.state || {}), arthSetuStep: nextStep }, '');
    setStep(nextStep);
    window.scrollTo(0, 0);
  };

  const handleBack = () => {
    if (step === 'intro') return;
    window.history.back();
  };

  const getStepNumber = () => {
    switch (step) {
      case 'purpose': return 1;
      case 'about': return 2;
      case 'financial': return 3;
      case 'location': return 4;
      case 'specifics': return 5;
      case 'review': return 6;
      default: return 0;
    }
  };

  const handleStateChange = async (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const name = STATES.find((s) => s.code === code)?.name || '';
    setFormData((prev: any) => ({ ...prev, state: code, stateName: name, district: '', districtName: '', dynamicAnswers: {} }));
    setAvailableDistricts([]);
    setLocationError('');
    if (!code) return;
    setIsLoadingDistricts(true);
    try {
      const districts = await fetchDistrictsFromBackend(code);
      setAvailableDistricts(districts);
      if (districts.length === 0) setLocationError('Verified district data is not available for this State/UT yet.');
    } catch (error: any) {
      setLocationError(error?.message || 'Verified district data could not be loaded.');
    } finally {
      setIsLoadingDistricts(false);
    }
  };

  const handleDistrictChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const code = e.target.value;
    const name = availableDistricts.find((d) => d.code === code)?.name || '';
    setFormData((prev: any) => ({ ...prev, district: code, districtName: name, dynamicAnswers: {} }));
  };

  const renderIntro = () => (
    <div className="max-w-2xl mx-auto text-center mt-12 mb-24 px-4">
      <div className="inline-flex items-center gap-2 bg-soft-teal text-secondary px-3 py-1.5 rounded-full font-semibold text-xs tracking-wide mb-6">SMART SCHEME RECOMMENDER</div>
      <h1 className="text-3xl md:text-5xl font-extrabold text-primary leading-tight mb-6">Find Government Schemes That Actually Match You</h1>
      <p className="text-lg text-text-muted leading-relaxed mb-8">Answer only the details needed by verified Government eligibility rules. ArthSetu will show source-backed schemes and the correct official application route.</p>
      <div className="flex flex-col items-center gap-3 mb-10">
        <span className="text-[14px] font-semibold text-text-main bg-gray-100 px-4 py-2 rounded-full">Usually takes a few minutes</span>
        <div className="flex items-center gap-1.5 text-text-muted text-[13px]"><ShieldCheck className="w-4 h-4 text-secondary" /><span>No Aadhaar or document upload required for discovery</span></div>
      </div>
      <button onClick={() => goToStep('purpose')} className="bg-primary text-white px-10 py-4 rounded-[12px] font-bold text-[16px] hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg transition-all flex items-center justify-center gap-2 mx-auto w-full sm:w-auto">Start <ArrowRight className="w-5 h-5" /></button>
    </div>
  );

  const renderPurpose = () => (
    <div className="max-w-4xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-2 text-center">What do you need support for?</h2>
      <p className="text-center text-text-muted text-sm mb-8">This only narrows relevant scheme families. Final eligibility comes from official scheme rules.</p>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        {PURPOSES.map((item) => (
          <button key={item.id} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, purpose: item.id }))}
            className={`p-5 rounded-[18px] border-2 text-left cursor-pointer transition-all hover:-translate-y-0.5 hover:shadow-md ${formData.purpose === item.id ? 'border-secondary bg-soft-teal' : 'border-gray-100 bg-white hover:border-gray-200'}`}>
            <item.icon className={`w-6 h-6 mb-3 ${formData.purpose === item.id ? 'text-secondary' : 'text-text-muted'}`} />
            <div className={`font-bold text-[14px] mb-1 ${formData.purpose === item.id ? 'text-secondary' : 'text-primary'}`}>{item.title}</div>
            <div className="text-[12px] text-text-muted leading-relaxed">{item.desc}</div>
          </button>
        ))}
      </div>
      <div className="mt-10 flex justify-end">
        <button disabled={!formData.purpose} onClick={() => goToStep('about')}
          className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center justify-center gap-2 w-full sm:w-auto ${formData.purpose ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          Continue <ArrowRight className="w-5 h-5" />
        </button>
      </div>
    </div>
  );

  const renderAbout = () => {
    const dob = formData.dob ? new Date(`${formData.dob}T00:00:00`) : null;
    const isValidDob = !!dob && !Number.isNaN(dob.getTime()) && dob <= new Date();
    const categories = [
      { id: 'SC', label: 'Scheduled Caste (SC)' },
      { id: 'ST', label: 'Scheduled Tribe (ST)' },
      { id: 'OBC', label: 'Other Backward Class (OBC)' },
      { id: 'General', label: 'General / Open Category' },
    ];
    const canContinue = !!formData.beneficiaryType && isValidDob && !!formData.gender && !!formData.category;

    return (
      <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
        <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Tell us about yourself</h2>
        <div className="bg-white rounded-[22px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-7">
          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-3">I am applying for</label>
            <div className="flex flex-wrap gap-2">
              {['Myself', 'Dependent / Child', 'Family Member'].map((val) => (
                <button key={val} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, beneficiaryType: val }))}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.beneficiaryType === val ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
              ))}
            </div>
          </div>

          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Date of Birth</label>
            <input type="date" value={formData.dob} max={new Date().toISOString().split('T')[0]}
              onChange={(e) => setFormData((prev: any) => ({ ...prev, dob: e.target.value }))}
              className="w-full sm:w-1/2 bg-white border border-gray-200 rounded-xl py-3 px-4 text-[14px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
          </div>

          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Gender</label>
            <div className="flex flex-wrap gap-2">
              {['Male', 'Female', 'Other'].map((val) => (
                <button key={val} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, gender: val }))}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.gender === val ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{val}</button>
              ))}
            </div>
          </div>

          <div>
            <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Social Category</label>
            <div className="flex flex-wrap gap-2">
              {categories.map((cat) => (
                <button key={cat.id} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, category: cat.id }))}
                  className={`px-4 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.category === cat.id ? 'bg-secondary text-white border-secondary shadow-sm' : 'bg-white text-text-muted border-gray-200 hover:border-gray-300'}`}>{cat.label}</button>
              ))}
            </div>
          </div>

          <div>
            <button type="button" onClick={() => setShowOptionalAttributes((v) => !v)} className="flex items-center gap-2 text-[13px] font-semibold text-secondary hover:text-primary transition-colors">
              {showOptionalAttributes ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
              Additional profile details — only use when relevant to a scheme
            </button>
          </div>

          {showOptionalAttributes && (
            <div className="flex flex-col gap-5 pt-4 border-t border-gray-100">
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Person with Disability (PwD)</label>
                <div className="flex gap-2">
                  {['Yes', 'No'].map((val) => {
                    const yes = val === 'Yes';
                    return <button key={val} type="button" onClick={() => setFormData((prev: any) => {
                      const dynamicAnswers = { ...prev.dynamicAnswers };
                      if (!yes) delete dynamicAnswers.DisabilityPercentage;
                      return { ...prev, isPwD: yes, dynamicAnswers };
                    })} className={`px-5 py-2.5 rounded-full text-[13px] font-semibold border transition-all ${formData.isPwD === yes ? 'bg-secondary text-white border-secondary' : 'bg-white text-text-muted border-gray-200'}`}>{val}</button>;
                  })}
                </div>
              </div>
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">EWS</label>
                <div className="flex gap-2">{['Yes', 'No'].map((val) => <button key={val} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, isEws: val === 'Yes' }))} className={`px-5 py-2.5 rounded-full text-[13px] font-semibold border ${formData.isEws === (val === 'Yes') ? 'bg-secondary text-white border-secondary' : 'bg-white text-text-muted border-gray-200'}`}>{val}</button>)}</div>
              </div>
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Minority Community</label>
                <div className="flex gap-2">{['Yes', 'No'].map((val) => <button key={val} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, isMinority: val === 'Yes' }))} className={`px-5 py-2.5 rounded-full text-[13px] font-semibold border ${formData.isMinority === (val === 'Yes') ? 'bg-secondary text-white border-secondary' : 'bg-white text-text-muted border-gray-200'}`}>{val}</button>)}</div>
              </div>
              <div>
                <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Ex-Serviceman</label>
                <div className="flex gap-2">{['Yes', 'No'].map((val) => <button key={val} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, isExServiceman: val === 'Yes' }))} className={`px-5 py-2.5 rounded-full text-[13px] font-semibold border ${formData.isExServiceman === (val === 'Yes') ? 'bg-secondary text-white border-secondary' : 'bg-white text-text-muted border-gray-200'}`}>{val}</button>)}</div>
              </div>
            </div>
          )}
        </div>
        <div className="mt-8 flex justify-end">
          <button disabled={!canContinue} onClick={() => goToStep('financial')}
            className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center gap-2 ${canContinue ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
            Continue <ArrowRight className="w-5 h-5" />
          </button>
        </div>
      </div>
    );
  };

  const renderFinancial = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Financial Information</h2>
      <div className="bg-white rounded-[22px] p-6 md:p-8 border border-gray-100 shadow-sm">
        <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">Annual Family Income (₹)</label>
        <div className="relative">
          <span className="absolute left-4 top-1/2 -translate-y-1/2 text-[16px] font-bold text-text-muted">₹</span>
          <input type="text" placeholder="e.g. 300000"
            value={formData.income ? new Intl.NumberFormat('en-IN').format(Number(String(formData.income).replace(/\D/g, ''))) : ''}
            onChange={(e) => setFormData((prev: any) => ({ ...prev, income: e.target.value.replace(/\D/g, '') }))}
            className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 pl-8 pr-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary" />
        </div>
        <p className="text-[12px] text-text-muted mt-2 flex items-center gap-1"><Info className="w-3.5 h-3.5" /> Used only where an official scheme has an income rule.</p>
      </div>
      <div className="mt-10 flex justify-end">
        <button disabled={!formData.income} onClick={() => goToStep('location')}
          className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center gap-2 ${formData.income ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          Continue <ArrowRight className="w-5 h-5" />
        </button>
      </div>
    </div>
  );

  const renderLocation = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Where are you located?</h2>
      <div className="bg-white rounded-[22px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-6">
        <div>
          <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">State / UT</label>
          <select value={formData.state} onChange={handleStateChange} className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary appearance-none">
            <option value="">Select State / UT</option>
            {STATES.map((s) => <option key={s.code} value={s.code}>{s.name}</option>)}
          </select>
        </div>
        <div>
          <label className="block text-[13px] font-bold text-text-muted uppercase tracking-wide mb-2">District</label>
          <select value={formData.district} onChange={handleDistrictChange} disabled={!formData.state || isLoadingDistricts || availableDistricts.length === 0}
            className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary appearance-none disabled:opacity-50 disabled:cursor-not-allowed">
            <option value="">{isLoadingDistricts ? 'Loading verified districts…' : 'Select district'}</option>
            {availableDistricts.map((district) => <option key={district.code} value={district.code}>{district.name}</option>)}
          </select>
          {locationError && <div className="mt-3 flex items-start gap-2 text-[12px] text-amber-700 bg-amber-50 border border-amber-100 rounded-lg px-3 py-2"><AlertCircle className="w-4 h-4 mt-0.5" />{locationError}</div>}
        </div>
      </div>
      <div className="mt-10 flex justify-end">
        <button disabled={!formData.state || !formData.district || isFetchingQuestions} onClick={async () => {
          setIsFetchingQuestions(true);
          setSchemeError('');
          try {
            const questions = await fetchDynamicQuestions(formData);
            setDynamicQuestions(questions);
            goToStep(questions.length > 0 ? 'specifics' : 'review');
          } catch (error: any) {
            setSchemeError(error?.message || 'Eligibility questions could not be loaded.');
          } finally {
            setIsFetchingQuestions(false);
          }
        }} className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center gap-2 ${formData.state && formData.district && !isFetchingQuestions ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>
          {isFetchingQuestions ? 'Checking rules…' : <>Continue <ArrowRight className="w-5 h-5" /></>}
        </button>
      </div>
      {schemeError && <div className="mt-4 text-right text-sm text-red-600">{schemeError}</div>}
    </div>
  );

  const renderSpecifics = () => {
    const isPwDNo = formData.isPwD === false || formData.dynamicAnswers?.IsPwD === 'false';
    const activeQuestions = dynamicQuestions.filter((q) => !(q.id === 'DisabilityPercentage' && isPwDNo));
    const isComplete = activeQuestions.every((q) => {
      const val = formData.dynamicAnswers[q.id];
      if (q.type === 'taxonomy') return !!val?.activity && (val.activity !== 'Other' || !!val.customActivityText?.trim());
      if (q.id === 'DisabilityPercentage') {
        const num = Number(val);
        return val !== '' && val != null && !Number.isNaN(num) && num >= 0 && num <= 100;
      }
      return val !== '' && val != null;
    });

    return (
      <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
        <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-2 text-center">Additional Details</h2>
        <p className="text-center text-sm text-text-muted mb-8">Only questions required by unresolved mandatory rules in your current verified scheme candidates are shown.</p>
        <div className="bg-white rounded-[22px] p-6 md:p-8 border border-gray-100 shadow-sm flex flex-col gap-7">
          {activeQuestions.length === 0 ? <p className="text-center text-text-muted py-6">No additional eligibility details are required.</p> : activeQuestions.map((q) => (
            <div key={q.id}>
              <label className="block text-[15px] font-bold text-primary mb-3">{q.label}</label>
              {q.type === 'single_choice' && <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">{q.options?.map((opt) => (
                <button key={opt.value} type="button" onClick={() => {
                  if (q.id === 'IsPwD') {
                    const isNo = opt.value === 'false';
                    const dynamicAnswers = { ...formData.dynamicAnswers, IsPwD: opt.value };
                    if (isNo) delete dynamicAnswers.DisabilityPercentage;
                    setFormData((prev: any) => ({ ...prev, isPwD: !isNo, dynamicAnswers }));
                  } else {
                    setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: opt.value } }));
                  }
                }} className={`py-3 px-4 rounded-xl font-bold text-[14px] border-2 transition-all text-left ${formData.dynamicAnswers[q.id] === opt.value ? 'border-secondary bg-soft-teal text-secondary' : 'border-gray-100 bg-white text-text-muted hover:border-gray-200'}`}>{opt.label}</button>
              ))}</div>}
              {q.type === 'yes_no' && <div className="flex gap-3">{['Yes', 'No'].map((opt) => <button key={opt} type="button" onClick={() => setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: opt } }))} className={`flex-1 py-3 px-6 rounded-xl font-bold text-[14px] border-2 ${formData.dynamicAnswers[q.id] === opt ? 'border-secondary bg-soft-teal text-secondary' : 'border-gray-100 text-text-muted'}`}>{opt}</button>)}</div>}
              {q.type === 'text' && <input type="text" value={formData.dynamicAnswers[q.id] || ''} onChange={(e) => setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: e.target.value } }))} className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary" />}
              {q.type === 'numeric' && <input type="number" min={q.id === 'DisabilityPercentage' ? 0 : undefined} max={q.id === 'DisabilityPercentage' ? 100 : undefined} value={formData.dynamicAnswers[q.id] || ''} onChange={(e) => setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: e.target.value } }))} className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[15px] text-primary focus:outline-none focus:border-secondary" />}
              {q.type === 'currency' && <div className="relative"><span className="absolute left-4 top-1/2 -translate-y-1/2 font-bold text-text-muted">₹</span><input type="text" value={formData.dynamicAnswers[q.id] ? new Intl.NumberFormat('en-IN').format(Number(formData.dynamicAnswers[q.id])) : ''} onChange={(e) => setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: e.target.value.replace(/\D/g, '') } }))} className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 pl-8 pr-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary" /></div>}
              {q.type === 'taxonomy' && <TaxonomySelector value={formData.dynamicAnswers[q.id] || { activity: '', customActivityText: '' }} onChange={(val) => setFormData((prev: any) => ({ ...prev, dynamicAnswers: { ...prev.dynamicAnswers, [q.id]: val } }))} />}
              {q.helpText && <div className="mt-2 text-[12px] text-text-muted flex items-start gap-1.5"><Info className="w-4 h-4 mt-0.5 text-blue-400" /><span>{q.helpText}</span></div>}
            </div>
          ))}
        </div>
        <div className="mt-10 flex justify-end"><button disabled={!isComplete} onClick={() => goToStep('review')} className={`px-10 py-4 rounded-[12px] font-bold text-[16px] transition-all flex items-center gap-2 ${isComplete ? 'bg-primary text-white hover:bg-primary/90 hover:-translate-y-0.5 hover:shadow-lg' : 'bg-gray-100 text-gray-400 cursor-not-allowed'}`}>Continue <ArrowRight className="w-5 h-5" /></button></div>
      </div>
    );
  };

  const activeReviewQuestions = dynamicQuestions.filter((q) => !(q.id === 'DisabilityPercentage' && formData.isPwD === false));

  const renderReview = () => (
    <div className="max-w-2xl mx-auto mt-8 mb-24 px-4">
      <h2 className="text-2xl md:text-3xl font-extrabold text-primary mb-8 text-center">Review Your Profile</h2>
      <div className="bg-white rounded-[22px] border border-gray-100 shadow-sm overflow-hidden divide-y divide-gray-50">
        <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Purpose</span><span className="text-right font-semibold text-primary">{PURPOSES.find((p) => p.id === formData.purpose)?.title}</span></div>
        <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Beneficiary</span><span className="text-right font-semibold text-primary">{formData.beneficiaryType}</span></div>
        <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Profile</span><span className="text-right font-semibold text-primary">{formData.gender} · {formData.category}<br /><span className="text-xs text-text-muted">DOB: {formData.dob}</span></span></div>
        <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Income</span><span className="font-semibold text-primary">₹{new Intl.NumberFormat('en-IN').format(Number(formData.income || 0))}</span></div>
        <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Location</span><span className="text-right font-semibold text-primary">{formData.districtName}, {formData.stateName}</span></div>
        {formData.isPwD !== undefined && <div className="px-6 py-5 flex justify-between gap-4"><span className="font-bold text-primary">Person with Disability (PwD)</span><span className="font-semibold text-primary">{formData.isPwD ? 'Yes' : 'No'}</span></div>}
        {activeReviewQuestions.length > 0 && <div className="px-6 py-5">
          <div className="flex justify-between mb-4"><span className="font-bold text-primary">Additional Details</span><button type="button" onClick={() => goToStep('specifics')} className="text-secondary font-semibold text-sm">Edit</button></div>
          <div className="space-y-3">{activeReviewQuestions.map((q) => {
            let displayValue: any = formData.dynamicAnswers[q.id];
            if (q.id === 'IsPwD') displayValue = formData.isPwD === true ? 'Yes' : formData.isPwD === false ? 'No' : displayValue;
            if (q.type === 'taxonomy' && displayValue) displayValue = displayValue.activity === 'Other' ? displayValue.customActivityText : ACTIVITY_CATEGORIES.find((c) => c.id === displayValue.activity)?.title || displayValue.activity;
            if (q.type === 'currency' && displayValue) displayValue = `₹${new Intl.NumberFormat('en-IN').format(Number(displayValue))}`;
            if (!displayValue && q.id === 'DisabilityPercentage' && !formData.isPwD) return null;
            return <div key={q.id}><div className="text-[11px] uppercase tracking-wide font-bold text-text-muted">{q.label}</div><div className="font-semibold text-primary">{displayValue || 'Not provided'}</div></div>;
          })}</div>
        </div>}
      </div>
      <div className="mt-8 text-center">
        <p className="text-[13px] text-text-muted mb-6">Only current, source-backed and verified Government scheme records are eligible for citizen results.</p>
        <button onClick={async () => {
          setSchemeError('');
          setStep('processing');
          window.scrollTo(0, 0);
          const sanitizedAnswers: Record<string, string> = {};
          dynamicQuestions.forEach((q) => {
            const value = formData.dynamicAnswers?.[q.id];
            if (value == null || value === '') return;
            if (q.type === 'taxonomy') sanitizedAnswers[q.id] = value.activity === 'Other' ? value.customActivityText : value.activity;
            else if (q.type === 'numeric' || q.type === 'currency') sanitizedAnswers[q.id] = String(value).replace(/[^0-9.]/g, '');
            else sanitizedAnswers[q.id] = String(value);
          });
          const disabilityPercentage = sanitizedAnswers.DisabilityPercentage ? Number(sanitizedAnswers.DisabilityPercentage) : undefined;
          const payload = { ...formData, income: String(formData.income || '').replace(/[^0-9.]/g, ''), dynamicAnswers: sanitizedAnswers, disabilityPercentage };
          try {
            const res = await fetch(`${apiBase()}/api/schemes/match`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(payload) });
            if (!res.ok) throw new Error('The verified scheme service is currently unavailable.');
            const data = await res.json();
            setSchemeResults(data);
            window.history.pushState({ ...(window.history.state || {}), arthSetuStep: 'result' }, '');
            setStep('result');
            window.scrollTo(0, 0);
          } catch (error: any) {
            setSchemeError(error?.message || 'Unable to load verified scheme results. Please try again.');
            window.history.replaceState({ ...(window.history.state || {}), arthSetuStep: 'review' }, '');
            setStep('review');
          }
        }} className="bg-secondary text-white px-10 py-4 rounded-[12px] font-bold text-[16px] hover:bg-secondary/90 hover:-translate-y-0.5 hover:shadow-lg transition-all inline-flex items-center gap-2">Find My Schemes <Search className="w-5 h-5" /></button>
        {schemeError && <div className="mt-4 text-sm text-red-600">{schemeError}</div>}
      </div>
    </div>
  );

  const renderProcessing = () => (
    <div className="max-w-2xl mx-auto text-center mt-24 mb-24 px-4"><div className="flex flex-col items-center gap-6"><div className="w-16 h-16 rounded-full border-4 border-secondary border-t-transparent animate-spin" /><div><h2 className="text-2xl font-extrabold text-primary mb-2">Checking Verified Schemes</h2><p className="text-text-muted text-[15px]">Evaluating your profile against source-backed Government rules…</p></div></div></div>
  );

  const statusClass = (status: string) => {
    const s = (status || '').toUpperCase();
    if (s === 'OPEN') return 'bg-green-50 text-green-700 border-green-100';
    if (s === 'CLOSED') return 'bg-red-50 text-red-700 border-red-100';
    if (s === 'NOT_YET_OPEN') return 'bg-amber-50 text-amber-700 border-amber-100';
    return 'bg-gray-50 text-gray-600 border-gray-100';
  };

  const renderResult = () => {
    if (!schemeResults) return null;
    const recommended = schemeResults.recommended || [];
    return (
      <div className="max-w-5xl mx-auto mt-4 mb-24 px-4">
        <div className="mb-8">
          <button onClick={handleBack} className="inline-flex items-center gap-2 text-[14px] font-bold text-text-muted hover:text-primary transition-colors mb-6"><ArrowLeft className="w-4 h-4" />Back to Review</button>
          <div className="text-center">
            <h2 className="text-3xl md:text-4xl font-extrabold text-primary mb-3">Your Scheme Results</h2>
            <p className="text-[15px] text-text-muted max-w-2xl mx-auto">These results come from the verified production dataset. Discovery, demo, placeholder and unverified records are excluded.</p>
          </div>
        </div>

        {recommended.length === 0 && <div className="bg-white border border-gray-100 rounded-[20px] p-8 text-center shadow-sm"><h3 className="font-bold text-primary text-lg mb-2">No verified eligible scheme found for the information provided</h3><p className="text-text-muted text-sm">This does not mean no Government scheme exists. Try updating your profile or check schemes that need more information below.</p></div>}

        {recommended.length > 0 && <div className="space-y-5 mb-10">
          <div className="flex items-end justify-between gap-4"><div><h3 className="text-xl font-extrabold text-primary">Eligible Schemes</h3><p className="text-sm text-text-muted mt-1">{recommended.length} source-backed result{recommended.length === 1 ? '' : 's'}</p></div></div>
          {recommended.map((scheme: any) => {
            const canApply = !!scheme.applicationUrl && (scheme.applicationStatus === 'OPEN' || scheme.applicationStatus === 'NOT_APPLICABLE');
            return <article key={scheme.id} className="bg-white rounded-[20px] border border-gray-100 shadow-sm hover:shadow-md transition-shadow overflow-hidden">
              <div className="p-6 md:p-7">
                <div className="flex flex-col md:flex-row md:items-start md:justify-between gap-4">
                  <div className="min-w-0">
                    <div className="flex flex-wrap items-center gap-2 mb-2">
                      <span className="text-[11px] font-bold uppercase tracking-wide bg-soft-teal text-secondary px-2.5 py-1 rounded-full">Eligible</span>
                      {scheme.governmentLevel && <span className="text-[11px] font-bold uppercase tracking-wide bg-blue-50 text-blue-700 px-2.5 py-1 rounded-full">{scheme.governmentLevel}</span>}
                      {scheme.applicationStatus && <span className={`text-[11px] font-bold uppercase tracking-wide border px-2.5 py-1 rounded-full ${statusClass(scheme.applicationStatus)}`}>{String(scheme.applicationStatus).replaceAll('_', ' ')}</span>}
                    </div>
                    <h4 className="text-xl font-extrabold text-primary leading-snug">{scheme.name}</h4>
                    <p className="text-[13px] text-text-muted mt-1">{scheme.owningAuthority || [scheme.ministry, scheme.department].filter(Boolean).join(' · ')}</p>
                  </div>
                  {canApply && <a href={scheme.applicationUrl} target="_blank" rel="noreferrer" className="shrink-0 inline-flex items-center justify-center bg-secondary text-white px-5 py-3 rounded-xl font-bold text-sm hover:bg-secondary/90 transition-colors">{scheme.applicationRoute || 'Apply on Official Portal'} <ArrowRight className="w-4 h-4 ml-2" /></a>}
                </div>

                {scheme.description && <p className="text-[14px] text-text-muted leading-relaxed mt-4 max-w-3xl">{scheme.description}</p>}
                <div className="flex flex-wrap gap-2 mt-4">
                  {scheme.benefitType && <span className="text-[11px] font-bold bg-gray-100 text-text-muted px-3 py-1.5 rounded-full">{scheme.benefitType}</span>}
                  {!canApply && scheme.applicationRoute && <span className="text-[11px] font-bold bg-primary/5 text-primary px-3 py-1.5 rounded-full">{scheme.applicationRoute}</span>}
                </div>

                <div className="mt-5 pt-4 border-t border-gray-100 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 text-[12px] text-text-muted">
                  <div className="flex items-center gap-2"><ShieldCheck className="w-4 h-4 text-secondary" /><span>Verified source: <strong className="text-text-main">{scheme.officialSource}</strong></span></div>
                  <div>Last verified: <strong className="text-text-main">{scheme.lastVerified || 'Date not available'}</strong></div>
                </div>
                <div className="mt-3 flex flex-wrap gap-3 text-[12px]">
                  {scheme.sourceUrl && <a href={scheme.sourceUrl} target="_blank" rel="noreferrer" className="font-semibold text-secondary hover:text-primary">View official source</a>}
                  {scheme.applicationUrl && !canApply && <a href={scheme.applicationUrl} target="_blank" rel="noreferrer" className="font-semibold text-secondary hover:text-primary">View official application information</a>}
                </div>
              </div>
              <button onClick={() => setExpandedDetails((prev) => ({ ...prev, [scheme.id]: !prev[scheme.id] }))} className="w-full px-6 py-4 border-t border-gray-100 flex items-center justify-between text-[13px] font-semibold text-text-main hover:bg-gray-50">
                <span>Why this scheme matched</span><ChevronDown className={`w-4 h-4 transition-transform ${expandedDetails[scheme.id] ? 'rotate-180' : ''}`} />
              </button>
              {expandedDetails[scheme.id] && <div className="px-6 py-4 bg-gray-50/70 space-y-3">{scheme.ruleComparisons?.filter((r: any) => r.status === 'Matched').map((r: any, idx: number) => <div key={idx} className="grid sm:grid-cols-3 gap-2 text-[12px] border-b border-gray-100 pb-3 last:border-0"><strong>{r.ruleName}</strong><span>Your value: {r.userValue}</span><span>Requirement: {r.schemeCondition}</span></div>)}</div>}
            </article>;
          })}
        </div>}

        {schemeResults.moreInfoNeeded?.length > 0 && <div className="border border-gray-100 rounded-[18px] overflow-hidden bg-white mt-8"><button onClick={() => setExpandedMoreInfo((v) => !v)} className="w-full px-5 py-4 flex items-center justify-between font-bold text-primary"><span>More Information Needed ({schemeResults.moreInfoNeeded.length})</span><ChevronDown className={`w-5 h-5 transition-transform ${expandedMoreInfo ? 'rotate-180' : ''}`} /></button>{expandedMoreInfo && <div className="px-5 pb-5 border-t border-gray-50 divide-y divide-gray-50">{schemeResults.moreInfoNeeded.map((scheme: any) => <div key={scheme.id} className="py-4"><div className="font-bold text-primary">{scheme.name}</div><div className="text-sm text-amber-700 mt-1">Needed to decide eligibility: {scheme.missingRules?.join(', ') || 'additional verified rule information'}</div></div>)}</div>}</div>}

        {schemeResults.notEligible?.length > 0 && <div className="border border-gray-100 rounded-[18px] overflow-hidden bg-white mt-6"><button onClick={() => setExpandedAlternative((v) => !v)} className="w-full px-5 py-4 flex items-center justify-between font-bold text-primary"><span>Not Eligible ({schemeResults.notEligible.length})</span><ChevronDown className={`w-5 h-5 transition-transform ${expandedAlternative ? 'rotate-180' : ''}`} /></button>{expandedAlternative && <div className="px-5 pb-5 border-t border-gray-50 divide-y divide-gray-50">{schemeResults.notEligible.map((scheme: any) => <div key={scheme.id} className="py-4"><div className="font-bold text-primary">{scheme.name}</div><button onClick={() => setExpandedNotEligible((prev) => ({ ...prev, [scheme.id]: !prev[scheme.id] }))} className="text-xs font-bold text-red-600 mt-2 inline-flex items-center gap-1">Why not eligible <ChevronDown className="w-3 h-3" /></button>{expandedNotEligible[scheme.id] && <div className="mt-3 bg-red-50 rounded-lg p-3">{scheme.ruleComparisons?.filter((r: any) => r.status === 'Failed').map((r: any, idx: number) => <div key={idx} className="flex gap-2 py-2 text-xs"><XCircle className="w-4 h-4 text-red-600 shrink-0" /><span><strong>{r.ruleName}</strong>: your value {r.userValue}; scheme requires {r.schemeCondition}</span></div>)}</div>}</div>)}</div>}</div>}

        <div className="mt-8 flex flex-col sm:flex-row gap-3">
          <button onClick={handleBack} className="flex-1 inline-flex items-center justify-center gap-2 bg-white text-primary border border-primary/20 px-6 py-3.5 rounded-xl font-bold hover:border-primary"><ArrowLeft className="w-4 h-4" />Back to Review</button>
          <button onClick={() => {
            setFormData({ purpose: '', category: '', dob: '', income: '', state: '', stateName: '', district: '', districtName: '', beneficiaryType: 'Myself', dynamicAnswers: {} });
            setDynamicQuestions([]);
            setSchemeResults(null);
            window.history.pushState({ arthSetuStep: 'intro' }, '');
            setStep('intro');
          }} className="flex-1 bg-primary text-white px-6 py-3.5 rounded-xl font-bold">Start New Search</button>
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-[calc(100vh-160px)] bg-bg-main w-full py-8 md:py-12">
      {step !== 'intro' && step !== 'processing' && step !== 'result' && <div className="max-w-2xl mx-auto px-4 mb-10">
        <button onClick={handleBack} className="flex items-center gap-1.5 text-text-muted font-bold hover:text-primary transition-colors mb-4 text-[14px]"><ArrowLeft className="w-4 h-4" />Back</button>
        <div className="flex items-center justify-between mb-2"><span className="text-[12px] font-bold text-text-muted uppercase tracking-wider">Step {getStepNumber()} of 6</span><span className="text-[12px] font-bold text-primary">{step === 'purpose' ? 'Purpose' : step === 'about' ? 'About You' : step === 'financial' ? 'Financial' : step === 'location' ? 'Location' : step === 'specifics' ? 'Additional Details' : 'Review'}</span></div>
        <div className="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden"><div className="h-full bg-secondary transition-all duration-500 rounded-full" style={{ width: `${(getStepNumber() / 6) * 100}%` }} /></div>
      </div>}
      {step === 'intro' && renderIntro()}
      {step === 'purpose' && renderPurpose()}
      {step === 'about' && renderAbout()}
      {step === 'financial' && renderFinancial()}
      {step === 'location' && renderLocation()}
      {step === 'specifics' && renderSpecifics()}
      {step === 'review' && renderReview()}
      {step === 'processing' && renderProcessing()}
      {step === 'result' && renderResult()}
    </div>
  );
}
