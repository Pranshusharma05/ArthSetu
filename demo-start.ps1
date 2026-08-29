Write-Host "Starting ArthSetu Demo..."
Write-Host "Starting ASP.NET Backend..."
Start-Process powershell -ArgumentList "-NoExit -Command "cd ArthSetuBackend; dotnet run""

Write-Host "Waiting for Backend to start (5 seconds)..."
Start-Sleep -Seconds 5

Write-Host "Starting React Frontend..."
Start-Process powershell -ArgumentList "-NoExit -Command "npm run dev""

Write-Host "Opening Browser..."
Start-Process "http://localhost:3000"

Write-Host "ArthSetu Demo is running!"
Write-Host "Backend: http://localhost:5000"
Write-Host "Frontend: http://localhost:3000"
