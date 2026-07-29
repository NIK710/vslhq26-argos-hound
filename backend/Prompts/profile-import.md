# ArgosHound Builder Profile Export

Create a narrow work-profile summary for use in an opportunity scouting application.
Include only information I have shared about my projects, products, current skills,
learning goals, professional interests, preferred opportunity types, general location,
and effort preferences.

Do not include private conversations, credentials, financial information, health
information, protected characteristics, family details, or facts you are uncertain
about. Do not infer missing personal details. Ask me to review the result before I share
it with another service.

Return only one JSON object using this exact shape:

```json
{
  "name": "My preferred name",
  "currentSkills": ["skill"],
  "learningGoals": ["goal"],
  "interests": ["interest"],
  "preferredOpportunityTypes": ["type"],
  "location": "Optional city or region, or null",
  "effortPreferences": ["preference"]
}
```
