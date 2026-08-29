const fs = require('fs');

// Fix FindScheme.tsx
let uiContent = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

uiContent = uiContent.replace('export default function FindScheme() {', `
const TaxonomySelector = ({ value, onChange }: { value: any, onChange: (val: any) => void }) => {
  return (
    <div className="space-y-4">
      <select 
        value={value.activity} 
        onChange={(e) => onChange({ ...value, activity: e.target.value })}
        className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
      >
        <option value="">Select an activity</option>
        {ACTIVITY_CATEGORIES.map(c => (
          <option key={c.id} value={c.id}>{c.title}</option>
        ))}
      </select>
      {value.activity === 'Other' && (
        <input 
          type="text" 
          placeholder="Please specify" 
          value={value.customActivityText || ''}
          onChange={(e) => onChange({ ...value, customActivityText: e.target.value })}
          className="w-full bg-gray-50 border border-gray-200 rounded-xl py-3.5 px-4 text-[16px] font-bold text-primary focus:outline-none focus:border-secondary focus:ring-1 focus:ring-secondary"
        />
      )}
    </div>
  );
};

export function FindScheme() {
`);

const missingStates = `  const [showAllCategories, setShowAllCategories] = useState(false);
  const [showOptionalAttributes, setShowOptionalAttributes] = useState(false);
  const [isLoadingDistricts, setIsLoadingDistricts] = useState(false);
  const [expandedDetails, setExpandedDetails] = useState<Record<string, boolean>>({});
  const [expandedMoreInfo, setExpandedMoreInfo] = useState<Record<string, boolean>>({});
  const [expandedAlternative, setExpandedAlternative] = useState<Record<string, boolean>>({});
`;

uiContent = uiContent.replace(/const \[isIncomeConfirmed, setIsIncomeConfirmed\] = useState\(false\);/, 
  'const [isIncomeConfirmed, setIsIncomeConfirmed] = useState(false);\n' + missingStates
);

// Fix loading districts manually.
uiContent = uiContent.replace('const districts = await fetchDistrictsFromBackend(code);', `
    setIsLoadingDistricts(true);
    const districts = await fetchDistrictsFromBackend(code);
    setIsLoadingDistricts(false);
`);

fs.writeFileSync('src/pages/FindScheme.tsx', uiContent, 'utf-8');

// Fix server.ts
let serverContent = fs.readFileSync('server.ts', 'utf-8');
serverContent = serverContent.replace('const stateName = STATES_MAP[state] || \'State\';', 'const stateName = STATES_MAP[state as string] || \'State\';');
fs.writeFileSync('server.ts', serverContent, 'utf-8');
