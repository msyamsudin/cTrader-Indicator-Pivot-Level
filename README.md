# Pivot Level

Indikator ini akan menggambar level pivot menggunakan metode perhitungan yang bisa dipilih oleh pengguna. Metode perhitungan yang tersedia adalah sebagai berikut:

# Perhitungan Level Pivot untuk Berbagai Metode

#### H = High, L = Low, C = Close, O = Open (jika diperlukan), k = koefisien spesifik per metode

### Classic / Standard
PP = (H + L + C) / 3 | R = (2 * PP) - L + k * (H - L) | S = (2 * PP) - H - k * (H - L)
##### k = 0 untuk R1/S1, k = 1 untuk R2/S2

### Fibonacci
PP = (H + L + C) / 3 | R = PP + k * (H - L) | S = PP - k * (H - L)
##### k = 0.382 untuk R1/S1, k = 0.618 untuk R2/S2, k = 1.000 untuk R3/S3

### Camarilla
PP = (H + L + C) / 3 | R = C + (H - L) * k | S = C - (H - L) * k
##### k = 1.1/12 untuk R1/S1, k = 1.1/6 untuk R2/S2, k = 1.1/4 untuk R3/S3, k = 1.1/2 untuk R4/S4

### Woodie
PP = (H + L + 2 * C) / 4 | R = (2 * PP) - L + k * (H - L) | S = (2 * PP) - H - k * (H - L)
##### k = 0 untuk R1/S1, k = 1 untuk R2/S2

### DeMark
X = (C < O) ? (H + 2 * L + C) : (C > O) ? (2 * H + L + C) : (H + L + 2 * C) | PP = X / 4 | R = X / 2 - L | S = X / 2 - H
##### Hanya 1 level R/S, tidak ada k

### Catatan:
- Classic: Sederhana dan umum digunakan
- Fibonacci: Menggunakan rasio Fibonacci
- Camarilla: Cocok untuk trading jangka pendek
- Woodie: Fokus pada harga penutupan
- DeMark: Prediktif, hanya 1 level R dan S

![Image](https://github.com/user-attachments/assets/62115a36-d182-4e4d-ba28-fbe46cc9294e)

![Image](https://github.com/user-attachments/assets/af826cf0-512c-433a-983b-1c661bf2b68b)