const fs = require('fs');
let content = fs.readFileSync('src/components/Navbar.tsx', 'utf-8');

const target = `        {/* Left: Logo */}
        <div className="flex items-center gap-3">
          <div className="relative w-10 h-10 bg-soft-blue rounded-[14px] flex items-center justify-center text-primary font-bold text-xl overflow-hidden border border-blue-100">
            {/* Minimal Bridge Symbol */}
            <div className="absolute bottom-2 w-6 h-4 border-t-4 border-l-4 border-r-4 border-primary rounded-t-lg"></div>
            <div className="absolute bottom-2 w-full h-[2px] bg-primary"></div>
          </div>
          <div className="flex flex-col justify-center">
            <span className="text-primary font-bold text-xl leading-none tracking-tight">ArthSetu</span>
            <span className="text-text-muted text-[11px] font-medium hidden sm:block mt-0.5">Bridging People to Opportunities</span>
          </div>
        </div>`;

const replacement = `        {/* Left: Logo */}
        <Link 
          to="/" 
          aria-label="Go to ArthSetu Home" 
          className="flex items-center gap-3 hover:opacity-80 transition-opacity focus:outline-none focus-visible:ring-2 focus-visible:ring-primary rounded-lg p-1 -ml-1"
        >
          <div className="relative w-10 h-10 bg-soft-blue rounded-[14px] flex items-center justify-center text-primary font-bold text-xl overflow-hidden border border-blue-100">
            {/* Minimal Bridge Symbol */}
            <div className="absolute bottom-2 w-6 h-4 border-t-4 border-l-4 border-r-4 border-primary rounded-t-lg"></div>
            <div className="absolute bottom-2 w-full h-[2px] bg-primary"></div>
          </div>
          <div className="flex flex-col justify-center">
            <span className="text-primary font-bold text-xl leading-none tracking-tight">ArthSetu</span>
            <span className="text-text-muted text-[11px] font-medium hidden sm:block mt-0.5">Bridging People to Opportunities</span>
          </div>
        </Link>`;

content = content.replace(target, replacement);
fs.writeFileSync('src/components/Navbar.tsx', content, 'utf-8');
