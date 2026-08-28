export function displayInitials(value: string | null | undefined): string {
  const name = value?.trim();
  if (!name) {
    return '?';
  }

  const parts = name.split(/\s+/).filter(Boolean);
  if (parts.length >= 2) {
    return `${parts[0]![0]!}${parts[1]![0]!}`.toUpperCase();
  }

  return name.slice(0, 2).toUpperCase();
}
