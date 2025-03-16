# Pivot Level
![Image](https://github.com/user-attachments/assets/62115a36-d182-4e4d-ba28-fbe46cc9294e)

![Image](https://github.com/user-attachments/assets/af826cf0-512c-433a-983b-1c661bf2b68b)
Indikator ini akan menggambar level pivot menggunakan metode perhitungan yang bisa dipilih oleh pengguna. Metode perhitungan yang tersedia adalah sebagai berikut:

# Perhitungan Level Pivot untuk Berbagai Metode
# H = High, L = Low, C = Close, O = Open (jika diperlukan)

## 1. Classic / Standard Pivot Points
PP = (H + L + C) / 3
R1 = (2 * PP) - L
S1 = (2 * PP) - H
R2 = PP + (H - L)
S2 = PP - (H - L)

## 2. Fibonacci Pivot Points
PP = (H + L + C) / 3
R1 = PP + 0.382 * (H - L)
R2 = PP + 0.618 * (H - L)
R3 = PP + 1.000 * (H - L)
S1 = PP - 0.382 * (H - L)
S2 = PP - 0.618 * (H - L)
S3 = PP - 1.000 * (H - L)

## 3. Camarilla Pivot Points
PP = (H + L + C) / 3
R1 = C + (H - L) * 1.1 / 12
R2 = C + (H - L) * 1.1 / 6
R3 = C + (H - L) * 1.1 / 4
R4 = C + (H - L) * 1.1 / 2
S1 = C - (H - L) * 1.1 / 12
S2 = C - (H - L) * 1.1 / 6
S3 = C - (H - L) * 1.1 / 4
S4 = C - (H - L) * 1.1 / 2

## 4. Woodie Pivot Points
PP = (H + L + 2 * C) / 4
R1 = (2 * PP) - L
R2 = PP + (H - L)
S1 = (2 * PP) - H
S2 = PP - (H - L)

## 5. DeMark Pivot Points
# Tentukan X berdasarkan kondisi
if C < O:
    X = H + (2 * L) + C
elif C > O:
    X = (2 * H) + L + C
else:  # C == O
    X = H + L + (2 * C)

PP = X / 4
R1 = X / 2 - L
S1 = X / 2 - H

# Catatan:
# - Classic: Sederhana dan umum digunakan
# - Fibonacci: Menggunakan rasio Fibonacci
# - Camarilla: Cocok untuk trading jangka pendek
# - Woodie: Fokus pada harga penutupan
# - DeMark: Prediktif, hanya 1 level R dan S