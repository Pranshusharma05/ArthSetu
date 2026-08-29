const fs = require('fs');
let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

const correctCategories = `const ACTIVITY_CATEGORIES = [
  { id: 'Agriculture', title: 'Agriculture', keywords: ['farming', 'crop', 'seeds', 'tractor'] },
  { id: 'Dairy', title: 'Dairy / Livestock', keywords: ['milk', 'cow', 'buffalo', 'cattle', 'animal'] },
  { id: 'Poultry', title: 'Poultry', keywords: ['chicken', 'egg', 'bird', 'hen'] },
  { id: 'Fisheries', title: 'Fisheries', keywords: ['fish', 'pond', 'aqua', 'shrimp'] },
  { id: 'Food Processing', title: 'Food Processing', keywords: ['pickle', 'jam', 'packaging', 'mill'] },
  { id: 'Handloom', title: 'Handloom', keywords: ['weaving', 'loom', 'fabric'] },
  { id: 'Handicrafts', title: 'Handicrafts', keywords: ['pottery', 'craft', 'artisan', 'woodwork'] },
  { id: 'Textiles', title: 'Textiles / Garments', keywords: ['tailoring', 'clothes', 'stitching', 'apparel'] },
  { id: 'Manufacturing', title: 'Manufacturing', keywords: ['factory', 'plant', 'production', 'making'] },
  { id: 'Trading', title: 'Trading / Retail', keywords: ['shop', 'store', 'grocery', 'trading', 'wholesale', 'sell'] },
  { id: 'Professional', title: 'Professional Services', keywords: ['consulting', 'ca', 'lawyer', 'clinic'] },
  { id: 'Repair', title: 'Repair & Maintenance', keywords: ['mobile repair', 'garage', 'mechanic', 'workshop', 'fixing'] },
  { id: 'Transport', title: 'Transport / Logistics', keywords: ['taxi', 'truck', 'auto', 'delivery', 'transport', 'cab'] },
  { id: 'Tourism', title: 'Tourism / Hospitality', keywords: ['hotel', 'restaurant', 'cafe', 'tourism', 'travel'] },
  { id: 'Digital', title: 'Digital / IT', keywords: ['computer', 'software', 'printing', 'xerox'] },
  { id: 'Construction', title: 'Construction', keywords: ['builder', 'contractor', 'materials', 'hardware'] },
  { id: 'SelfEmployment', title: 'Self-Employment', keywords: ['freelance', 'independent', 'individual'] },
  { id: 'SHG', title: 'SHG / Group Enterprise', keywords: ['self help group', 'women group', 'cooperative'] },
  { id: 'Startup', title: 'Startup', keywords: ['startup', 'innovation', 'tech', 'new'] },
  { id: 'Other', title: 'Other', keywords: [] }
];`;

content = content.replace(/const ACTIVITY_CATEGORIES = \[\s*\{[\s\S]*?\];/g, correctCategories);

fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
