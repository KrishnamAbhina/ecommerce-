# Product Delete Cascade Logic

## Delete Flow

When you delete a product, this is the cascade order:

```
DELETE Product #1
    ↓
1. Find all CartItems with ProductId = 1
    ↓
2. Delete those CartItems
    ↓
3. Check each affected Cart
    - If Cart has 0 items → DELETE the Cart
    - If Cart has items → KEEP the Cart
    ↓
4. Find all OrderItems with ProductId = 1
    ↓
5. Delete those OrderItems
    ↓
6. Check each affected Order
    - If Order has 0 items → DELETE the Order
    - If Order has items → KEEP the Order
    ↓
7. DELETE the Product
```

## Real-World Examples

### Example 1: Delete Product from Single-Item Cart
```
Before:
- User has Cart #5 with:
  - CartItem: Product #1 (qty: 2)

DELETE /api/product/id/1

After:
- Cart #5: DELETED (no items left)
- Product #1: DELETED
```

### Example 2: Delete Product from Multi-Item Cart
```
Before:
- User has Cart #5 with:
  - CartItem: Product #1 (qty: 2)
  - CartItem: Product #2 (qty: 1)

DELETE /api/product/id/1

After:
- Cart #5: KEPT (still has Product #2)
  - CartItem: Product #2 (qty: 1) [remaining]
- Product #1: DELETED
```

### Example 3: Delete Product from Orders
```
Before:
- Order #10 with:
  - OrderItem: Product #1 (qty: 1)
  
- Order #11 with:
  - OrderItem: Product #1 (qty: 1)
  - OrderItem: Product #2 (qty: 2)

DELETE /api/product/id/1

After:
- Order #10: DELETED (no items left)
- Order #11: KEPT (still has Product #2)
  - OrderItem: Product #2 (qty: 2) [remaining]
- Product #1: DELETED
```

## Database Integrity
✅ No orphaned Carts (carts with no items)
✅ No orphaned Orders (orders with no items)
✅ All related records cleaned up automatically
✅ Database stays consistent

## Code Logic
```csharp
// For each cart affected by deleting cart items:
if (cart has 0 items remaining)
    delete the cart
else
    keep the cart

// For each order affected by deleting order items:
if (order has 0 items remaining)
    delete the order
else
    keep the order
```
