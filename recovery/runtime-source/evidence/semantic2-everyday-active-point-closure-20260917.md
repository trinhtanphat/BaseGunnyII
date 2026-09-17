# Semantic recovery — everyday active-point packet closure (2026-09-17)

## Base and scope
- Base branch: `recovery/runtime-semantic-phase2-20260915`.
- Base SHA: `d422a350b55808995dcee939ce6a32a15f6f1392`.
- Scope restores `EverydayActivePointHandler` and the exact `SendExpBlessedData(int)` packet contract end-to-end.
- No `master` merge, production deploy, DLL copy, service restart, or force push is part of this wave.

## TDD and qualification
- RED guard failed all 5 closure contracts before source changes.
- Detached qualification worktree at the exact base SHA built `Game.Server` with 0 compiler errors.
- GREEN guard passes handler source, project mapping, interface contract, concrete packet serializer, and console implementation.

## Semantic parity
- `EverydayActivePointHandler` matches recovered Road source after namespace-only normalization.
- `AbstractPacketLib.SendExpBlessedData` exact-token semantics: packet 155, player id, byte 8, int 0, then `SendTCP`.
- `ConsolePacketLib.SendExpBlessedData` matches recovered runtime and throws `NotImplementedException`.
- `IPacketLib` carries the exact `void SendExpBlessedData(int PlayerId)` contract.
- Independent parity verifier: all checks TRUE.

## Fresh verification
- Toolchain: .NET SDK 8.0.425 MSBuild/Roslyn with net35 reference root.
- Actual recovery worktree `Game.Server`: exit 0, compiler errors 0.
- Safety scan: sensitive hits 0; decompiler/artifact hits 0.
- `git -c core.whitespace=cr-at-eol diff --check`: PASS.

## Fresh inventory delta
- Runtime types: 1551; runtime members: 10639; recovered C# files: 2334.
- Runtime-only types: 196 -> 195.
- Canonical-present types: 720 -> 721.
- Variant-conflict types: 59 -> 59.
- Runtime-only members: 2389 -> 2385.
- Canonical-present members: 5838 -> 5842.
- Variant-conflict members: 176 -> 176.