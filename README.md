# SocietyLedger - Premium Society Management Platform

A comprehensive, modern web application for apartment society management with maintenance collection, billing, payment tracking, expense management, and detailed reporting.

## 🎨 Features

### ✅ Implemented Pages

1. **Landing Page**
   - Premium hero section with gradient backgrounds
   - Feature showcase with icons and descriptions
   - Pricing plans (Starter, Standard, Pro)
   - FAQ section
   - Professional footer

2. **Authentication**
   - Login with email/password
   - Signup with society creation
   - Forgot password flow
   - Premium form design with gradient backgrounds

3. **Dashboard**
   - KPI cards with statistics (Total Flats, Due, Collected, Balance)
   - Income vs Expense charts (Bar & Line charts)
   - Recent activity feed
   - Quick action cards
   - Fully responsive with animations

4. **Flats Management**
   - Searchable flat list with filters
   - CSV import functionality with preview
   - Individual flat ledger view
   - Outstanding balance tracking
   - Bulk actions support

5. **Billing & Invoices**
   - Bill generation wizard
   - Status filters (Paid, Pending, Overdue, Partial)
   - Bill preview and PDF download
   - Email sending capability
   - Collection rate tracking

6. **Payments**
   - Record payments with multiple modes (Cash, UPI, Cheque, Bank Transfer, Card)
   - Payment history with search
   - Receipt upload support
   - Payment mode distribution analytics
   - Reference number tracking

7. **Expenses**
   - Category-based expense tracking (Electricity, Water, Security, Repairs, Salary, Others)
   - Visual pie chart breakdown
   - Vendor management
   - Receipt attachments
   - Monthly expense analysis

8. **Reports & Analytics**
   - Income vs Expense trend charts
   - Collection rate analysis
   - Outstanding by flat report
   - Exportable reports (PDF/CSV)
   - Custom date range selection
   - AGM report generation

9. **Users & Access Management**
   - Team member invitation
   - Role-based access (Admin, Treasurer, Member)
   - User status management
   - Activity tracking

10. **Settings**
    - Society information management
    - Bank details configuration
    - Notification preferences
    - Security settings
    - Billing cycle configuration

### 🎯 UI/UX Enhancements

- **Premium Design**: Gradient backgrounds, rounded corners, premium shadows
- **Color Scheme**: Teal (primary) + Indigo (secondary) for professional finance-grade appearance
- **Typography**: Inter for body, Poppins for headings
- **Dark Mode**: Full dark mode support with theme toggle
- **Animations**: Smooth transitions, fade-ins, slide-ups, scale effects
- **Responsive**: Mobile-first design with breakpoints for all screen sizes
- **Mobile FAB**: Floating action button for quick actions on mobile
- **Micro-interactions**: Hover effects, loading states, smooth transitions

### 🗄️ Database (Supabase)

Complete schema with Row Level Security:

- **societies** - Society information
- **users** - User management with role-based access
- **flats** - Flat/unit management
- **bills** - Bill generation and tracking
- **payments** - Payment records with multiple modes
- **expenses** - Expense tracking with categories

All tables have:
- RLS policies for data isolation
- Proper indexes for performance
- Foreign key relationships
- Audit timestamps

## 🚀 Tech Stack

- **Frontend**: React 18 + TypeScript
- **Build Tool**: Vite
- **Styling**: Tailwind CSS with custom theme
- **State Management**: React Query (TanStack Query)
- **Forms**: React Hook Form + Zod validation
- **Charts**: Recharts
- **Animations**: Framer Motion
- **Database**: Supabase (PostgreSQL)
- **Icons**: Lucide React
- **HTTP Client**: Axios

## 📦 Project Structure

```
src/
├── components/
│   ├── layout/
│   │   ├── DashboardLayout.tsx
│   │   ├── Navbar.tsx
│   │   └── Sidebar.tsx
│   └── ui/
│       ├── Badge.tsx
│       ├── Button.tsx
│       ├── Card.tsx
│       ├── Input.tsx
│       ├── Modal.tsx
│       ├── Select.tsx
│       ├── Table.tsx
│       ├── Toast.tsx
│       └── MobileFAB.tsx
├── contexts/
│   └── ThemeContext.tsx
├── hooks/
│   └── api/
│       ├── useAuth.ts
│       ├── useBilling.ts
│       ├── useDashboard.ts
│       ├── useExpenses.ts
│       └── useFlats.ts
├── lib/
│   ├── api.ts
│   ├── supabase.ts
│   └── utils.ts
├── pages/
│   ├── Billing.tsx
│   ├── Dashboard.tsx
│   ├── Expenses.tsx
│   ├── Flats.tsx
│   ├── ForgotPassword.tsx
│   ├── LandingPage.tsx
│   ├── Login.tsx
│   ├── Payments.tsx
│   ├── Reports.tsx
│   ├── Settings.tsx
│   ├── Signup.tsx
│   └── Users.tsx
├── types/
│   └── index.ts
├── App.tsx
├── Router.tsx
└── main.tsx
```

## 🛠️ Setup Instructions

1. **Install Dependencies**
   ```bash
   npm install
   ```

2. **Environment Variables**
   Create a `.env` file with:
   ```env
   VITE_SUPABASE_URL=your_supabase_url
   VITE_SUPABASE_ANON_KEY=your_supabase_anon_key
   VITE_API_BASE_URL=http://localhost:3000/api
   ```

3. **Run Development Server**
   ```bash
   npm run dev
   ```

4. **Build for Production**
   ```bash
   npm run build
   ```

## 🎨 Design Principles

1. **Premium Look**: High-quality shadows, gradients, and animations
2. **Finance-Grade**: Professional color scheme suitable for financial management
3. **Consistent Spacing**: 8px spacing system throughout
4. **Clear Hierarchy**: Typography and visual weight create clear information hierarchy
5. **Accessible**: WCAG compliant color contrasts and keyboard navigation
6. **Mobile-First**: Responsive design with mobile-optimized interactions

## 🔐 Security Features

- Row Level Security (RLS) on all database tables
- JWT-based authentication
- Role-based access control (Admin, Treasurer, Member)
- Secure password handling
- API request/response interceptors
- CORS protection

## 📱 Mobile Experience

- Responsive breakpoints for all screen sizes
- Collapsible sidebar on mobile
- Mobile FAB for quick actions
- Touch-optimized interactions
- Optimized charts for small screens

## 🚧 Future Enhancements

- Real-time notifications with Supabase Realtime
- Email/SMS integration for bill reminders
- Document storage for receipts
- Multi-society management
- Mobile app (React Native)
- Payment gateway integration
- Advanced analytics dashboard
- Automated bill generation
- WhatsApp integration

## 📄 License

Proprietary - All rights reserved

## 🤝 Support

For support, email support@societyledger.com or visit our help center.

---

**Built with ❤️ for modern society management**