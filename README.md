<div align="center">
<img width="1200" height="475" alt="GHBanner" src="https://ai.google.dev/static/site-assets/images/share-ais-513315318.png" />
</div>

# Run and deploy your AI Studio app

This contains everything you need to run your app locally.

View your app in AI Studio: https://ai.studio/apps/216ac5c5-c422-4954-9d82-4ded99ccc9c5

## Run Locally

**Prerequisites:**  Node.js


1. Install dependencies:
   `npm install`
2. Set the `GEMINI_API_KEY` in [.env.local](.env.local) to your Gemini API key
3. Run the app:
   `npm run dev`

## HOW TO START ARTHSETU DEMO

To run the complete demo (Frontend + ASP.NET Backend + SQL Server):

1. Open a PowerShell terminal.
2. Navigate to the project root directory: cd C:\Users\Pranshu Sharma\Workspace\arthsetu
3. Run the startup script: .\demo-start.ps1

The script will automatically start the ASP.NET backend on http://localhost:5000 and the React frontend on http://localhost:3000.

**Required Environment Variables:**
- .env.development is included with VITE_API_BASE_URL=http://localhost:5000

**Database:**
- The backend automatically uses (localdb)\mssqllocaldb which is built into Visual Studio / Windows. No manual configuration is required.
