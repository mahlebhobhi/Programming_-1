# RaceDay API Endpoint Plan

This plan matches the RaceDay database design and covers the required functionality for authentication, user profiles, events, categories, event enrolments and results.

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | /api/auth/register | Creates a new RaceDay user account. | Public | `{ firstName, lastName, email, password, phoneNumber, role }` | 201 Created - account created; 400 Bad Request - invalid data; 409 Conflict - email already exists |
| POST | /api/auth/login | Authenticates a user and returns a login token. | Public | `{ email, password }` | 200 OK - login successful; 401 Unauthorized - invalid login details |
| GET | /api/users/me | Returns the profile of the logged-in user. | Any logged-in user | None | 200 OK - profile returned; 401 Unauthorized - not logged in; 404 Not Found - profile not found |
| PUT | /api/users/me | Updates the logged-in user's profile details. | Any logged-in user | `{ firstName, lastName, phoneNumber }` | 200 OK - profile updated; 400 Bad Request - invalid data; 401 Unauthorized - not logged in |
| GET | /api/events | Returns a list of RaceDay events. | Public | None | 200 OK - list of events |
| GET | /api/events/{id} | Returns details for one event, including its categories. | Public | None | 200 OK - event returned; 404 Not Found - event does not exist |
| POST | /api/events | Creates a new running event. | Organiser | `{ eventName, eventDate, location, description, status }` | 201 Created - event created; 400 Bad Request - invalid data; 401 Unauthorized; 403 Forbidden |
| PUT | /api/events/{id} | Updates an existing event owned by the organiser. | Organiser | `{ eventName, eventDate, location, description, status }` | 200 OK - event updated; 404 Not Found; 403 Forbidden |
| DELETE | /api/events/{id} | Removes an event that has not been completed. | Organiser | None | 204 No Content; 404 Not Found; 409 Conflict - event cannot be removed |
| GET | /api/categories | Returns all available race categories. | Public | None | 200 OK - list of categories |
| GET | /api/events/{id}/categories | Returns categories available for a specific event. | Public | None | 200 OK - categories returned; 404 Not Found - event not found |
| POST | /api/events/{id}/categories | Adds a category to an event. | Organiser | `{ categoryId }` | 201 Created; 404 Not Found; 409 Conflict - category already assigned |
| DELETE | /api/events/{id}/categories/{categoryId} | Removes a category from an event when it has no active enrolments. | Organiser | None | 204 No Content; 404 Not Found; 409 Conflict |
| POST | /api/events/{id}/enrolments | Enrols the logged-in participant in an event category. | Participant | `{ categoryId }` | 201 Created; 404 Not Found; 409 Conflict - already enrolled or category unavailable |
| GET | /api/events/{id}/enrolments | Returns enrolments for an event. | Organiser | None | 200 OK - enrolment list; 403 Forbidden |
| GET | /api/users/me/enrolments | Returns the logged-in participant's enrolments. | Participant | None | 200 OK - enrolment list; 401 Unauthorized |
| DELETE | /api/enrolments/{id} | Cancels an enrolment. | Participant | None | 204 No Content; 404 Not Found; 409 Conflict |
| GET | /api/events/{id}/results | Returns the results for an event. | Public | None | 200 OK - results list; 404 Not Found |
| GET | /api/events/{id}/results/{categoryId} | Returns results for one category in an event. | Public | None | 200 OK - category results; 404 Not Found |
| POST | /api/enrolments/{id}/result | Records a result for a completed enrolment. | Organiser | `{ finishPosition, finishTimeSeconds, resultStatus }` | 201 Created; 400 Bad Request; 404 Not Found; 409 Conflict - result already exists |
| PUT | /api/enrolments/{id}/result | Corrects an existing result. | Organiser | `{ finishPosition, finishTimeSeconds, resultStatus }` | 200 OK; 404 Not Found; 403 Forbidden |
