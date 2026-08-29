import { Hero } from '../components/Hero';
import { Features } from '../components/Features';
import { HowItWorks } from '../components/HowItWorks';
import { TrustTransparency } from '../components/TrustTransparency';
import { FinalCTA } from '../components/FinalCTA';

export function Home() {
  return (
    <>
      <Hero />
      <Features />
      <HowItWorks />
      <TrustTransparency />
      <FinalCTA />
    </>
  );
}
