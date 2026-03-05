"use client"
import { TrendingUp, DollarSign, Home, AlertCircle, Plus, FileText, ArrowUpRight, ArrowDownLeft } from 'lucide-react'
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts"
import DashboardLayout from "../components/layout/DashboardLayout"
import { SubscriptionSummary } from "../components/SubscriptionSummary"

/**
 * Dashboard - Alytics-inspired redesign
 * Bold hero metrics, generous whitespace, rounded cards with soft shadows, micro-interactions
 */

export default function DashboardRedesigned() {
  const user = JSON.parse(localStorage.getItem("user") || "{}")

  const stats = [
    {
      title: "Total Collection",
      value: "₹12,50,000",
      change: "+12%",
      positive: true,
      icon: DollarSign,
    },
    {
      title: "Outstanding Dues",
      value: "₹4,50,000",
      change: "-8%",
      positive: true,
      icon: AlertCircle,
    },
    {
      title: "Total Flats",
      value: "120",
      change: "+4",
      positive: true,
      icon: Home,
    },
    {
      title: "Collection Rate",
      value: "85%",
      change: "+3%",
      positive: true,
      icon: TrendingUp,
    },
  ]

  const chartData = [
    { month: "Jul", income: 185000, expense: 65000 },
    { month: "Aug", income: 195000, expense: 72000 },
    { month: "Sep", income: 188000, expense: 58000 },
    { month: "Oct", income: 210000, expense: 82000 },
    { month: "Nov", income: 225000, expense: 55000 },
    { month: "Dec", income: 247000, expense: 48000 },
  ]

  const expenseData = [
    { name: "Maintenance", value: 380000, color: "hsl(var(--primary))" },
    { name: "Utilities", value: 220000, color: "hsl(var(--info))" },
    { name: "Security", value: 180000, color: "hsl(var(--success))" },
    { name: "Other", value: 85000, color: "hsl(var(--warning))" },
  ]

  const recentActivity = [
    { id: 1, type: "payment", description: "A-101 Maintenance Payment", amount: 5000, status: "completed" },
    { id: 2, type: "expense", description: "Electricity Bill", amount: -8500, status: "pending" },
    { id: 3, type: "payment", description: "B-205 Maintenance Payment", amount: 4500, status: "completed" },
    { id: 4, type: "bill", description: "January 2025 Bills Generated", amount: 0, status: "completed" },
    { id: 5, type: "expense", description: "Security Staff Salary", amount: -45000, status: "completed" },
  ]

  return (
    <DashboardLayout title="Dashboard">
      <div className="space-y-8">
        <div className="flex flex-col md:flex-row md:items-end md:justify-between gap-6">
          <div className="flex-1">
            <h5 className="hero-heading mb-2">Welcome back, {user.name || "User"}</h5>
            <p className="subheading">Here's a complete overview of your society's financials today.</p>
          </div>
          <button className="btn-primary w-full md:w-auto flex-shrink-0">
            <Plus className="w-5 h-5" />
            Quick Action
          </button>
        </div>

        <SubscriptionSummary />

        <div className="grid-auto-fit-lg">
          {stats.map((stat, index) => (
            <div
              key={index}
              className="card-base p-6 group card-hover animate-slide-in-up"
              style={{ animationDelay: `${0.05 * index}s` }}
            >
              <div className="flex items-start justify-between mb-6">
                <div className="w-12 h-12 gradient-primary rounded-lg flex items-center justify-center group-hover:scale-110 transition-transform duration-300">
                  <stat.icon className="w-6 h-6 text-white" />
                </div>
                <div
                  className={`flex items-center gap-1 text-sm font-semibold badge-base ${stat.positive ? 'badge-success' : 'badge-destructive'}`}
                >
                  {stat.positive ? <ArrowUpRight className="w-4 h-4" /> : <ArrowDownLeft className="w-4 h-4" />}
                  {stat.change}
                </div>
              </div>
              <p className="text-sm text-muted-foreground mb-2">{stat.title}</p>
              <p className="text-3xl font-bold text-foreground">{stat.value}</p>
            </div>
          ))}
        </div>

        <div className="grid lg:grid-cols-2 gap-8">
          {/* Income vs Expense */}
          <div className="card-base p-8 animate-slide-in-up" style={{ animationDelay: '0.2s' }}>
            <h3 className="text-xl font-bold text-foreground mb-6">Income vs Expense</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" opacity={0.3} />
                <XAxis dataKey="month" stroke="hsl(var(--muted-foreground))" />
                <YAxis stroke="hsl(var(--muted-foreground))" />
                <Tooltip
                  contentStyle={{
                    backgroundColor: "hsl(var(--card))",
                    border: "1px solid hsl(var(--border))",
                    borderRadius: "0.75rem",
                  }}
                  formatter={(value: number) => `₹${(value / 100000).toFixed(1)}L`}
                />
                <Legend />
                <Bar dataKey="income" fill="hsl(var(--primary))" radius={[8, 8, 0, 0]} />
                <Bar dataKey="expense" fill="hsl(var(--secondary))" radius={[8, 8, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>

          {/* Collection Trend */}
          <div className="card-base p-8 animate-slide-in-up" style={{ animationDelay: '0.25s' }}>
            <h3 className="text-xl font-bold text-foreground mb-6">Collection Trend</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={chartData}>
                <CartesianGrid strokeDasharray="3 3" stroke="hsl(var(--border))" opacity={0.3} />
                <XAxis dataKey="month" stroke="hsl(var(--muted-foreground))" />
                <YAxis stroke="hsl(var(--muted-foreground))" />
                <Tooltip
                  contentStyle={{
                    backgroundColor: "hsl(var(--card))",
                    border: "1px solid hsl(var(--border))",
                    borderRadius: "0.75rem",
                  }}
                  formatter={(value: number) => `₹${(value / 100000).toFixed(1)}L`}
                />
                <Line
                  type="monotone"
                  dataKey="income"
                  stroke="hsl(var(--primary))"
                  strokeWidth={3}
                  dot={{ fill: "hsl(var(--primary))", r: 6 }}
                  activeDot={{ r: 8 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Expense Breakdown */}
          <div className="card-base p-8 animate-slide-in-up" style={{ animationDelay: '0.3s' }}>
            <h3 className="text-xl font-bold text-foreground mb-6">Expense Breakdown</h3>
            <ResponsiveContainer width="100%" height={250}>
              <PieChart>
                <Pie
                  data={expenseData}
                  cx="50%"
                  cy="50%"
                  innerRadius={60}
                  outerRadius={90}
                  paddingAngle={2}
                  dataKey="value"
                >
                  {expenseData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={entry.color} />
                  ))}
                </Pie>
              </PieChart>
            </ResponsiveContainer>
            <div className="mt-6 space-y-3">
              {expenseData.map((item) => (
                <div key={item.name} className="flex items-center justify-between text-sm">
                  <div className="flex items-center gap-2">
                    <div className="w-3 h-3 rounded-full" style={{ backgroundColor: item.color }}></div>
                    <span className="text-muted-foreground">{item.name}</span>
                  </div>
                  <span className="font-semibold text-foreground">₹{(item.value / 100000).toFixed(1)}L</span>
                </div>
              ))}
            </div>
          </div>

          {/* Recent Activity */}
          <div className="card-base p-8 lg:col-span-2 animate-slide-in-up" style={{ animationDelay: '0.35s' }}>
            <div className="flex items-center justify-between mb-6">
              <h3 className="text-xl font-bold text-foreground">Recent Activity</h3>
              <a href="#" className="text-sm text-primary hover:text-primary font-semibold micro-interaction">
                View All →
              </a>
            </div>
            <div className="space-y-4">
              {recentActivity.map((activity) => (
                <div
                  key={activity.id}
                  className="flex items-center justify-between p-4 rounded-lg bg-muted/50 hover:bg-muted transition-colors micro-interaction"
                >
                  <div className="flex items-center gap-4">
                    <div
                      className={`w-10 h-10 rounded-lg flex items-center justify-center ${
                        activity.type === "payment"
                          ? "bg-success/10"
                          : activity.type === "expense"
                            ? "bg-destructive/10"
                            : "bg-info/10"
                      }`}
                    >
                      <FileText
                        className={`w-5 h-5 ${
                          activity.type === "payment"
                            ? "text-success"
                            : activity.type === "expense"
                              ? "text-destructive"
                              : "text-info"
                        }`}
                      />
                    </div>
                    <div>
                      <p className="font-semibold text-foreground text-sm">{activity.description}</p>
                      <p className="text-xs text-muted-foreground">Today</p>
                    </div>
                  </div>
                  {activity.amount !== 0 && (
                    <span
                      className={`font-semibold text-sm ${activity.amount > 0 ? "text-success" : "text-destructive"}`}
                    >
                      {activity.amount > 0 ? "+" : ""}₹{Math.abs(activity.amount).toLocaleString()}
                    </span>
                  )}
                </div>
              ))}
            </div>
          </div>
        </div>

        <div className="grid md:grid-cols-3 gap-6 mt-8">
          {[
            { title: "Generate Bills", desc: "Create monthly maintenance bills", icon: FileText },
            { title: "Record Payment", desc: "Add new payment entry", icon: DollarSign },
            { title: "Add Expense", desc: "Record society expenses", icon: AlertCircle },
          ].map((action, i) => (
            <button
              key={i}
              className="card-base p-6 group card-hover text-left animate-slide-in-up"
              style={{ animationDelay: `${0.4 + 0.05 * i}s` }}
            >
              <div className="w-12 h-12 gradient-primary rounded-lg flex items-center justify-center mb-4 group-hover:scale-110 transition-transform duration-300">
                <action.icon className="w-6 h-6 text-white" />
              </div>
              <h4 className="font-bold text-foreground mb-1">{action.title}</h4>
              <p className="text-sm text-muted-foreground">{action.desc}</p>
            </button>
          ))}
        </div>
      </div>
    </DashboardLayout>
  )
}
