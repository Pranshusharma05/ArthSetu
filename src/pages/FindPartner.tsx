import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Building2, MapPin, CheckCircle2 } from 'lucide-react';

export default function FindPartner() {
  const location = useLocation();
  const searchParams = new URLSearchParams(location.search);
  const schemeId = searchParams.get('schemeId');
  const [partners, setPartners] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPartners = async () => {
      try {
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
        const url = schemeId ? `/api/partners?schemeId=` : `/api/partners`;
        const res = await fetch(url);
        if (res.ok) {
          const data = await res.json();
          setPartners(data);
        }
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchPartners();
  }, [schemeId]);

  return (
    <div className="min-h-screen bg-gray-50 pt-24 pb-12">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Find Authorized Partner</h1>
          {schemeId && <p className="text-gray-600 mt-2">Showing authorized partners for scheme: {schemeId}</p>}
        </div>

        {loading ? (
          <div className="text-center py-12">Loading partners...</div>
        ) : partners.length === 0 ? (
          <div className="text-center py-12 bg-white rounded-xl shadow-sm border border-gray-100">
            <p className="text-gray-500">No authorized partners found.</p>
          </div>
        ) : (
          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {partners.map((p, idx) => (
              <div key={idx} className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 hover:shadow-md transition-shadow">
                <div className="flex items-start justify-between mb-4">
                  <div className="flex items-center gap-3">
                    <div className="w-12 h-12 bg-blue-50 rounded-lg flex items-center justify-center text-blue-600">
                      <Building2 size={24} />
                    </div>
                    <div>
                      <h3 className="font-bold text-gray-900">{p.name}</h3>
                      <p className="text-sm text-gray-500 flex items-center gap-1">
                        <MapPin size={14} /> {p.partnerType === 'HEAD_OFFICE' ? 'Official Channel Partner / Head Office' : p.partnerType}
                      </p>
                    </div>
                  </div>
                </div>
                
                <div className="space-y-3">
                  {p.partnerType === 'HEAD_OFFICE' && (
                    <div className="bg-yellow-50 text-yellow-800 text-xs px-3 py-2 rounded-md">
                      Beneficiary service location not yet verified
                    </div>
                  )}
                  
                  <div className="flex items-center gap-2 text-sm text-gray-600">
                    <span className="font-medium">Address:</span> {p.registeredAddress || 'N/A'}
                  </div>
                  <div className="flex items-center gap-2 text-sm text-gray-600">
                    <span className="font-medium">State:</span> {p.state || 'N/A'} {p.pincode ? '- ' + p.pincode : ''}
                  </div>
                </div>

                <div className="mt-4 pt-4 border-t border-gray-100 flex justify-between items-center text-sm">
                  <div className="flex items-center gap-1 text-green-600">
                    <CheckCircle2 size={14} />
                    <span className="font-medium">{p.verificationStatus}</span>
                  </div>
                  <span className="text-gray-400">Source: {p.sourceSnapshot}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

