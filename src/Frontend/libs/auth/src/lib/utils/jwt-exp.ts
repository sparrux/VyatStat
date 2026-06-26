/**
 * Reads JWT `exp` (seconds since epoch) from an access token without verifying the signature.
 * Used only to schedule proactive refresh; authorization remains server-side.
 */
export function getJwtExpirationUtcMs(accessToken: string): number | null {
  const parts = accessToken.split('.');
  if (parts.length < 2) {
    return null;
  }
  try {
    const payloadSegment = parts[1];
    const json = decodeBase64UrlPayload(payloadSegment);
    const payload = JSON.parse(json) as { exp?: unknown };
    if (typeof payload.exp !== 'number' || !Number.isFinite(payload.exp)) {
      return null;
    }
    return payload.exp * 1000;
  } catch {
    return null;
  }
}

function decodeBase64UrlPayload(segment: string): string {
  let base64 = segment.replace(/-/g, '+').replace(/_/g, '/');
  const pad = (4 - (base64.length % 4)) % 4;
  if (pad) {
    base64 += '='.repeat(pad);
  }
  const binary = atob(base64);
  const bytes = Uint8Array.from(binary, (c) => c.charCodeAt(0));
  return new TextDecoder('utf-8').decode(bytes);
}
