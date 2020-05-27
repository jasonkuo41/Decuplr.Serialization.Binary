

### Future Projections

#### Near future projections

- [ ] Implement explicit length for mutlidimesional array
- [ ] Implement T[,] T[,,] parsers

#### Further future projections

- [ ] Implement "True" Lazy\<T>

#### Furthest future projections
- [ ] Implement LazyBinaryReader
  - This allows use to do some cool stuffs like
  ```csharp=
    LazyBinaryReader.Read<UserInfo>(x => x.AnimeList)
  ```
  - This would not load the whole type. Instead,
  it would only serialize the target

  - We could even map this to a file, for example
  ```csharp=
    LazyBinaryReader.Read<UserInfo>("jason.bin", x => x.AnimeList)
  ```
- [ ] Implement LazyBinaryWriter
