"use client"

import { useState } from "react"
import { Save, Bell, Lock, Users, Palette, Database } from 'lucide-react'
import DashboardLayout from "../components/layout/DashboardLayout"
import Input from "../components/ui/Input"
import Button from "../components/ui/Button"

/**
 * Settings Page - Redesigned with Alytics aesthetic
 * Clean tabbed interface, generous spacing, organized sections
 */

export default function SettingsPageRedesigned() {
  const [activeTab, setActiveTab] = useState("general")

  const tabs = [
    { id: "general", label: "General", icon: Palette },
    { id: "notifications", label: "Notifications", icon: Bell },
    { id: "security", label: "Security", icon: Lock },
    { id: "team", label: "Team", icon: Users },
    { id: "data", label: "Data", icon: Database },
  ]

  return (
    <DashboardLayout title="Settings">
      <div className="space-y-8">
        <div>
          <h2 className="section-heading mb-2">Settings</h2>
          <p className="subheading">Manage your account and preferences</p>
        </div>

        <div className="border-b border-slate-200 dark:border-slate-800">
          <div className="flex gap-1 overflow-x-auto pb-0 -mb-px">
            {tabs.map((tab) => (
              <button
                key={tab.id}
                onClick={() => setActiveTab(tab.id)}
                className={`px-4 py-3 font-semibold text-sm transition-all whitespace-nowrap border-b-2 flex items-center gap-2 micro-interaction ${
                  activeTab === tab.id
                    ? "border-indigo-600 dark:border-indigo-500 text-indigo-600 dark:text-indigo-400"
                    : "border-transparent text-slate-600 dark:text-slate-400 hover:text-slate-900 dark:hover:text-slate-300"
                }`}
              >
                <tab.icon className="w-4 h-4" />
                {tab.label}
              </button>
            ))}
          </div>
        </div>

        <div className="space-y-6">
          {activeTab === "general" && (
            <div className="card-base p-8">
              <h3 className="text-xl font-bold text-foreground mb-8">Society Information</h3>
              <div className="space-y-6">
                <Input
                  label="Society Name"
                  placeholder="Enter society name"
                  defaultValue="Green Valley Complex"
                />
                <div className="grid md:grid-cols-2 gap-6">
                  <Input
                    label="Address"
                    placeholder="Enter address"
                    defaultValue="123 Main Street"
                  />
                  <Input
                    label="City"
                    placeholder="Enter city"
                    defaultValue="Mumbai"
                  />
                </div>
                <div className="pt-4">
                  <Button variant="primary" className="flex items-center gap-2">
                    <Save className="w-5 h-5" />
                    Save Changes
                  </Button>
                </div>
              </div>
            </div>
          )}

          {activeTab === "notifications" && (
            <div className="card-base p-8">
              <h3 className="text-xl font-bold text-foreground mb-8">Notification Preferences</h3>
              <div className="space-y-3">
                {[
                  { title: "Payment Reminders", desc: "Get reminded about pending payments" },
                  { title: "Bill Generated", desc: "Receive notification when bills are generated" },
                  { title: "Expense Updates", desc: "Get notified about new expenses" },
                  { title: "Reports", desc: "Receive monthly reports" },
                ].map((notif, i) => (
                  <label key={i} className="flex items-center justify-between p-4 rounded-lg bg-slate-100 dark:bg-slate-800/50 hover:bg-slate-200 dark:hover:bg-slate-700/50 transition-colors cursor-pointer micro-interaction">
                    <div>
                      <p className="font-semibold text-foreground">{notif.title}</p>
                      <p className="text-sm text-slate-600 dark:text-slate-400">{notif.desc}</p>
                    </div>
                    <input type="checkbox" defaultChecked className="w-5 h-5 accent-primary" />
                  </label>
                ))}
              </div>
            </div>
          )}

          {activeTab === "security" && (
            <div className="card-base p-8">
              <h3 className="text-xl font-bold text-foreground mb-8">Security Settings</h3>
              <div className="p-6 bg-destructive/10 border border-destructive/30 rounded-lg">
                <p className="font-semibold text-destructive mb-2">Danger Zone</p>
                <p className="text-sm text-slate-600 dark:text-slate-400 mb-4">
                  Change your password or enable two-factor authentication
                </p>
                <Button variant="outline" className="border-destructive text-destructive hover:bg-destructive/10">
                  Change Password
                </Button>
              </div>
            </div>
          )}

          {activeTab === "team" && (
            <div className="card-base p-8">
              <div className="flex items-center justify-between mb-8">
                <h3 className="text-xl font-bold text-foreground">Team Members</h3>
                <Button variant="primary" size="sm">
                  + Invite Member
                </Button>
              </div>
              <div className="space-y-3">
                {[
                  { name: "Rajesh Kumar", role: "Admin", email: "rajesh@example.com" },
                  { name: "Priya Singh", role: "Treasurer", email: "priya@example.com" },
                ].map((member, i) => (
                  <div key={i} className="flex items-center justify-between p-4 rounded-lg bg-slate-100 dark:bg-slate-800/50 hover:bg-slate-200 dark:hover:bg-slate-700/50 transition-colors micro-interaction">
                    <div>
                      <p className="font-semibold text-foreground">{member.name}</p>
                      <p className="text-sm text-muted-foreground">
                        {member.email} • {member.role}
                      </p>
                    </div>
                    <button className="text-muted-foreground hover:text-destructive transition-colors">×</button>
                  </div>
                ))}
              </div>
            </div>
          )}

          {activeTab === "data" && (
            <div className="card-base p-8">
              <h3 className="text-xl font-bold text-foreground mb-8">Data Management</h3>
              <div className="space-y-3">
                <button className="w-full p-4 card-interactive text-left">
                  <p className="font-semibold text-foreground">Export Data</p>
                  <p className="text-sm text-muted-foreground">Download all your data as CSV</p>
                </button>
                <button className="w-full p-4 card-interactive text-left">
                  <p className="font-semibold text-foreground">Backup Data</p>
                  <p className="text-sm text-muted-foreground">Create a manual backup of your data</p>
                </button>
              </div>
            </div>
          )}
        </div>
      </div>
    </DashboardLayout>
  )
}
