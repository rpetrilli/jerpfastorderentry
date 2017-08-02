ALTER TABLE vo_ordini_righe
  ADD COLUMN zpet_qta_ordinata numeric(14,4);
ALTER TABLE vo_ordini_righe
  ADD COLUMN zpet_qta_in_consegna numeric(14,4);
ALTER TABLE vo_ordini_righe
  ADD COLUMN zpet_sconto_1 numeric(14,4);
ALTER TABLE vo_ordini_righe
  ADD COLUMN zpet_sconto_2 numeric(14,4);
  ALTER TABLE vo_ordini_righe
  ADD COLUMN zpet_sconto_3 numeric(14,4);