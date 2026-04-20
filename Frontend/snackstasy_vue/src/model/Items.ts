export interface AllStands {
    "stand_id": number,
    "name": String,
    "pickup_id": number,
    "tablet_id": number
}

export interface ItemsByStand {
    "item_id": number,
    "stand_id": number,
    "name": String,
    "price": number,
    "stock": number
  }