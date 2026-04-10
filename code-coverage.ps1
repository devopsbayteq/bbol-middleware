# Limpiar resultados anteriores
Remove-Item -Recurse -Force ./TestResults
Remove-Item -Recurse -Force ./CoverageReport

# Ejecutar pruebas y generar cobertura
dotnet test --collect:"XPlat Code Coverage" --results-directory:"./TestResults"

# Generar informe combinado
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./CoverageReport" -reporttypes:Html

# Abrir el informe en el navegador
Start-Process "./CoverageReport/index.html"