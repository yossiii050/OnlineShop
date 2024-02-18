
## Add New Table to DataBAse

1. open class in Models.
2. in class add all pror with get,set methods
3. in DBUsers context add public DbSet<_name_> _name_ { get; set; }
4. run in pm console
```bash
  add-migration _explain_
```
5. run update
```bash
  update-database
```
