import test from 'node:test';
import assert from 'node:assert/strict';
import {
  MAX_FRAME_BYTES,
  resolveDestination,
  validateUpgradeRequest,
  validateBinaryFrame,
} from '../src/policy.mjs';

test('only route=game resolves to the fixed Gunny TCP destination', () => {
  assert.deepEqual(resolveDestination('game'), {
    hostname: '103.9.156.182',
    port: 9200,
  });
  assert.equal(resolveDestination('admin'), null);
  assert.equal(resolveDestination('103.9.156.182:22'), null);
});

test('socket endpoint requires GET websocket upgrade and route=game', () => {
  const ok = new Request('https://edge.test/socket?route=game', {
    headers: { Upgrade: 'websocket' },
  });
  assert.equal(validateUpgradeRequest(ok), null);
  assert.equal(validateUpgradeRequest(new Request('https://edge.test/socket?route=nope', {
    headers: { Upgrade: 'websocket' },
  })), 403);
  assert.equal(validateUpgradeRequest(new Request('https://edge.test/healthz')), 404);
  assert.equal(validateUpgradeRequest(new Request('https://edge.test/socket?route=game')), 426);
});

test('binary frames are bounded to 256 KiB and text is rejected', () => {
  assert.equal(MAX_FRAME_BYTES, 262144);
  assert.equal(validateBinaryFrame(new Uint8Array([1, 2, 3])), null);
  assert.equal(validateBinaryFrame(new Uint8Array(MAX_FRAME_BYTES)), null);
  assert.equal(validateBinaryFrame(new Uint8Array(MAX_FRAME_BYTES + 1)), 1009);
  assert.equal(validateBinaryFrame('text is not a Gunny TCP frame'), 1003);
});
