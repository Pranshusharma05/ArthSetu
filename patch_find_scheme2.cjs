const fs = require('fs');

let content = fs.readFileSync('src/pages/FindScheme.tsx', 'utf-8');

// The first patch script changed `<item.icon className="w-5 h-5" />` to `<Icon className="w-5 h-5" />`.
content = content.replace(/<Icon className="w-5 h-5" \/>/g, '<item.icon className="w-5 h-5" />');
content = content.replace(/<Icon className="w-4 h-4" \/>/g, '<item.icon className="w-4 h-4" />');

// Now let's change the inline map arrays.
content = content.replace(/\{\[\s*\{\s*id:\s*'Business',\s*icon:\s*Briefcase[\s\S]*?\}\]\.map\(\(item\)\s*=>\s*\(/, 
`{PURPOSES.slice(0, 6).map((item) => (`);

content = content.replace(/\{\[\s*\{\s*id:\s*'Banking',\s*icon:\s*Landmark[\s\S]*?\}\]\.map\(\(item\)\s*=>\s*\(/,
`{PURPOSES.slice(6).map((item) => (`);

// And we need to fix PURPOSES definition to use component references instead of strings
content = content.replace(/const PURPOSES = \[\s*\{\s*id:\s*'Agriculture',\s*icon:\s*'Map'/g,
`const PURPOSES = [
  { id: 'Agriculture', icon: Map,`);
content = content.replace(/'Landmark'/g, 'Landmark');
content = content.replace(/'Briefcase'/g, 'Briefcase');
content = content.replace(/'GraduationCap'/g, 'GraduationCap');
content = content.replace(/'Heart'/g, 'Heart');
content = content.replace(/'Home'/g, 'Home');
content = content.replace(/'Shield'/g, 'Shield');
content = content.replace(/'Monitor'/g, 'Monitor');
content = content.replace(/'Wrench'/g, 'Wrench');
content = content.replace(/'Users'/g, 'Users');
content = content.replace(/'Trophy'/g, 'Trophy');
content = content.replace(/'Bus'/g, 'Bus');
content = content.replace(/'MapPin'/g, 'MapPin');
content = content.replace(/'Droplet'/g, 'Droplet');
content = content.replace(/'Baby'/g, 'Baby');
content = content.replace(/'Search'/g, 'Search');
content = content.replace(/'Map'/g, 'Map');


fs.writeFileSync('src/pages/FindScheme.tsx', content, 'utf-8');
