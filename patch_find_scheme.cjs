const fs = require('fs');

let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const newPurposes = `const PURPOSES = [
  { id: 'Agriculture', icon: 'Map', title: 'Agriculture, Rural & Environment', desc: 'Farming, dairy, or agriculture activity' },
  { id: 'Banking', icon: 'Landmark', title: 'Banking, Financial Services & Insurance', desc: 'Financial support and insurance schemes' },
  { id: 'Business', icon: 'Briefcase', title: 'Business & Entrepreneurship', desc: 'Business, services, or manufacturing' },
  { id: 'Education', icon: 'GraduationCap', title: 'Education & Learning', desc: 'Higher education or professional courses' },
  { id: 'Health', icon: 'Heart', title: 'Health & Wellness', desc: 'Healthcare and medical assistance' },
  { id: 'Housing', icon: 'Home', title: 'Housing & Shelter', desc: 'Home construction or improvement' },
  { id: 'PublicSafety', icon: 'Shield', title: 'Public Safety, Law & Justice', desc: 'Legal and public safety schemes' },
  { id: 'Science', icon: 'Monitor', title: 'Science, IT & Communications', desc: 'Technology and science-related support' },
  { id: 'Skills', icon: 'Wrench', title: 'Skills & Employment', desc: 'Vocational training and skill development' },
  { id: 'SocialWelfare', icon: 'Users', title: 'Social Welfare & Empowerment', desc: 'Support for marginalized sections' },
  { id: 'Sports', icon: 'Trophy', title: 'Sports & Culture', desc: 'Athletics and cultural promotions' },
  { id: 'Transport', icon: 'Bus', title: 'Transport & Infrastructure', desc: 'Roads, transport and logistics' },
  { id: 'Travel', icon: 'MapPin', title: 'Travel & Tourism', desc: 'Hospitality and tourism support' },
  { id: 'Utility', icon: 'Droplet', title: 'Utility & Sanitation', desc: 'Water, sanitation, and utilities' },
  { id: 'WomenChild', icon: 'Baby', title: 'Women & Child', desc: 'Schemes dedicated to women and children' },
];`;

content = content.replace(
  "const ACTIVITY_CATEGORIES = [", 
  newPurposes + "\n\nconst ACTIVITY_CATEGORIES = ["
);

content = content.replace(
  /\{\[\s*\{\s*id:\s*'Business',\s*icon:\s*Briefcase[\s\S]*?\}\]\.map\(\(item\)\s*=>\s*\(/g,
  `{PURPOSES.slice(0, 6).map((item) => {
          const Icon = eval(item.icon);
          return (`
);

content = content.replace(
  /<item\.icon className="w-5 h-5" \/>/g,
  `<Icon className="w-5 h-5" />`
);

content = content.replace(
  /\{\[\s*\{\s*id:\s*'Banking',\s*icon:\s*Landmark[\s\S]*?\}\]\.map\(\(item\)\s*=>\s*\(/g,
  `{PURPOSES.slice(6).map((item) => {
            const Icon = eval(item.icon);
            return (`
);

content = content.replace(
  /<item\.icon className="w-4 h-4" \/>/g,
  `<Icon className="w-4 h-4" />`
);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
