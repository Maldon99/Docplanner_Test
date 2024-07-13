README

I used Visual studio community 2022 with .Net 8.0
I add a "ASP.NET Core Web API" (for the apis) 
I add a "Blazor for WebAssembly" projet (for the front)
I use xUnit 

How to test the app: 
The app use the api, so both projects must be running at the same time:
	Choose the solution node's context (right-click) menu and then choose Properties. 
	Choose the Multiple Startup Projects option and set appropriate actions.
	(Start: DoctorSlotAPP and DoctorSlotAPI)

To do that we allow cors.
The url's are in the appsettings.json file.

NOTE:
The field returned by the GetWeeklyAvailability: "facilityId" is not in the documentation
Seems that this field in required by the post:
In your Post json example there is not "Slot.FacilityId"
I add this field in the json in order to complete the post property.

