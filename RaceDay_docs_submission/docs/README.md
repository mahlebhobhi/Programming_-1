# RaceDay Planning Documents

## Files

- `RaceDay_Database.sql` - complete SQL Server database creation and seed script.
- `RaceDay_API_Endpoint_Plan.md` - API endpoint planning table.

## Database entities

The SQL script uses these eight entities:

1. UserAccount
2. Organiser
3. Participant
4. Event
5. Category
6. EventCategory
7. Enrolment
8. Result

## Main relationships

- UserAccount 1-to-0/1 Organiser
- UserAccount 1-to-0/1 Participant
- Organiser 1-to-many Event
- Event many-to-many Category through EventCategory
- Participant 1-to-many Enrolment
- EventCategory 1-to-many Enrolment
- Enrolment 1-to-0/1 Result

## Important design decision

`EventCategory` is used as a junction table because one event can offer several categories and one category can be offered at many events. The composite primary key `(EventID, CategoryID)` prevents the same category being added to the same event twice.

`Enrolment` references `(EventID, CategoryID)` together, so a participant can only enrol in a category that is actually available for that event.

## Testing

Run `RaceDay_Database.sql` in SQL Server Management Studio. The script creates `RaceDayDB`, creates all tables, inserts sample data, and runs SELECT queries at the end so the results can be checked.

The script is designed to be re-run because existing RaceDay tables are removed in dependency order before they are recreated.
