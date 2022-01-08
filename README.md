# librollback

Rollback is a feature of objects which allows them to return to a past state. Implementation of `IRollback` indicates a class supports rollback. Classes can make use of rollback-compatible data structure found within this library to more easily implement rollback.

The following classes represent `IRollback` implementations of data structures.

| Name | Description |
| ---- | ----------- |
| `RollbackList` | A list implementation which supports rollback. |
| `RollbackMap` | A map implementation which supports rollback. |
| `RollbackSet` | A set implementation which supports rollback. |
| `RollbackProperty` | A value reference which supports rollback. |
