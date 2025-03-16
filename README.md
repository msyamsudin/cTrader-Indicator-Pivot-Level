# cTrader Indicator: MultiPivotLevels

Indikator `MultiPivotLevels` dirancang untuk digunakan dengan cTrader dan menyediakan berbagai metode untuk menghitung dan menampilkan level pivot pada grafik. Indikator ini mendukung beberapa metode perhitungan pivot dan dapat menampilkan level pivot saat ini dan sebelumnya dengan warna yang dapat disesuaikan.

## Fitur

- Mendukung beberapa metode perhitungan pivot: Classic, Fibonacci, Camarilla, Woodie, dan DeMark.
- Dapat menampilkan level pivot untuk kerangka waktu yang ditentukan atau otomatis berdasarkan kerangka waktu grafik.
- Menampilkan level pivot saat ini dan sebelumnya dengan warna yang dapat disesuaikan.
- Opsi untuk menampilkan label harga pada level pivot.

## Parameter

- **Use Auto Timeframe** (bool): Menentukan apakah akan secara otomatis memilih kerangka waktu pivot berdasarkan kerangka waktu grafik. Default adalah `true`.
- **Pivot Timeframe** (TimeFrame): Menentukan kerangka waktu yang akan digunakan untuk perhitungan pivot ketika `Use Auto Timeframe` adalah `false`. Default adalah `Weekly`.
- **Pivot Method** (PivotMethod): Metode yang digunakan untuk menghitung level pivot. Default adalah `Classic`. Metode yang tersedia adalah:
  - `Classic`
  - `Fibonacci`
  - `Camarilla`
  - `Woodie`
  - `DeMark`
- **Levels Color** (Color): Warna yang digunakan untuk menggambar level pivot saat ini. Default adalah `Gray`.
- **Previous Levels Color** (Color): Warna yang digunakan untuk menggambar level pivot sebelumnya. Default adalah `DarkGray`.
- **Show Previous Levels** (int): Jumlah level pivot sebelumnya yang akan ditampilkan. Nilai minimum adalah `0`. Default adalah `1`.
- **Show Price Labels** (bool): Menentukan apakah akan menampilkan label harga untuk level pivot. Default adalah `true`.

## Cara Menggunakan

Untuk menggunakan indikator `MultiPivotLevels`, tambahkan ke grafik di cTrader dan konfigurasikan parameter sesuai kebutuhan. Indikator akan secara otomatis menghitung dan menampilkan level pivot berdasarkan metode dan kerangka waktu yang dipilih.

![Image](https://github.com/user-attachments/assets/62115a36-d182-4e4d-ba28-fbe46cc9294e)

![Image](https://github.com/user-attachments/assets/af826cf0-512c-433a-983b-1c661bf2b68b)