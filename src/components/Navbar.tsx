import { Menu, Globe, User } from "lucide-react";

import { Link } from 'react-router-dom';

export function Navbar() {
  return (
    <nav className="sticky top-0 z-50 w-full bg-white border-b border-gray-100 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-20 flex items-center justify-between">
        {/* Left: Logo */}
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
        </Link>

        {/* Middle: Desktop Nav */}
        <div className="hidden lg:flex items-center gap-8">
          <Link to="/" className="text-primary font-semibold hover:text-secondary transition-colors text-[15px]">Home</Link>
          <Link to="/find-scheme" className="text-text-muted font-medium hover:text-primary transition-colors text-[15px]">Find Scheme</Link>
          <Link to="/loan-planner" className="text-text-muted font-medium hover:text-primary transition-colors text-[15px]">Loan Planner</Link>
          <Link to="#" className="text-text-muted font-medium hover:text-primary transition-colors text-[15px]">Find Partner</Link>
          <Link to="#" className="text-text-muted font-medium hover:text-primary transition-colors text-[15px]">About</Link>
        </div>

        {/* Right: Actions */}
        <div className="flex items-center gap-4">
          <button className="hidden sm:flex items-center gap-1.5 text-text-muted hover:text-primary font-medium transition-colors text-[15px]">
            <Globe className="w-4 h-4" />
            <span>हिंदी | English</span>
          </button>
          <div className="hidden sm:block w-[1px] h-4 bg-gray-200 mx-2"></div>
          <button className="hidden sm:flex items-center gap-1.5 text-text-muted hover:text-primary font-medium transition-colors text-[15px]">
            <User className="w-4 h-4" />
            <span>Login</span>
          </button>
          <button className="bg-primary text-white px-5 py-2.5 rounded-[12px] font-semibold text-[15px] hover:-translate-y-0.5 hover:shadow-md transition-all hidden sm:block">
            Get Started
          </button>
          
          {/* Mobile Menu */}
          <button className="lg:hidden text-text-main p-2 hover:bg-gray-50 rounded-lg">
            <Menu className="w-6 h-6" />
          </button>
        </div>
      </div>
    </nav>
  );
}

