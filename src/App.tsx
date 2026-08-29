/**
 * @license
 * SPDX-License-Identifier: Apache-2.0
 */
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Navbar } from './components/Navbar';
import { Home } from './pages/Home';
import { FindScheme } from './pages/FindScheme';
import { LoanPlanner } from './pages/LoanPlanner';
import { Footer } from './components/Footer';
import { AIAssistantFloating } from './components/AIAssistantFloating';
import FindPartner from './pages/FindPartner';
import MyJourney from './pages/MyJourney';
import Admin from './pages/Admin';

export default function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col font-sans">
        <Navbar />
        <main className="flex-grow">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/find-scheme" element={<FindScheme />} />
            <Route path="/loan-planner" element={<LoanPlanner />} />
          <Route path="/find-partner" element={<FindPartner />} />
          <Route path="/my-journey" element={<MyJourney />} />
          <Route path="/admin" element={<Admin />} />
          </Routes>
        </main>
        <Footer />
        <AIAssistantFloating />
      </div>
    </BrowserRouter>
  );
}


