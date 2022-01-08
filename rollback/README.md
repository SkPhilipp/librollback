# Rollback

Rollback is a feature of objects which allows them to return to a past state. To enable instance-wide rollback, all
underlying entities and their fields part of an instance must support rollback. This is enabled through the following
classes;

| Name | Description |
| ---- | ----------- |
| `RollbackList` | A list implementation which supports rollback. |
| `RollbackMap` | A map implementation which supports rollback. |
| `RollbackSet` | A set implementation which supports rollback. |
| `RollbackProperty` | A value reference which supports rollback. |

Rollback-supporting objects should use the above classes as the basis for their implementation, and implement the `IRollback` or `IRollbackEmbedded` interface.

### Rollback Clock Embedding

Higher level objects, especially objects which are exposed to engine users should be responsible for keeping track of
the current time. This is done by referring to and embedding a `RollbackClock` object in the implementing class. The
`RollbackClock` object should be updated externally, usually at a single point in the engine.

See also:

- [Rollback.Tests/structures/EmbeddedObjectExample.cs](../Rollback.Tests/structures/EmbeddedObjectExample.cs)
- [Rollback.Tests/structures/EmbeddedObjectExampleTest.cs](../Rollback.Tests/structures/EmbeddedObjectExampleTest.cs)
