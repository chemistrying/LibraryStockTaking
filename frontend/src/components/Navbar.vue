<template>
    <nav :class="[`bg-${theme}`, `navbar-${theme}`, 'navbar', 'navbar-expand-lg']">
        <div class="container-fluid">
            <a class="navbar-brand" href="#" @click.prevent="returnHome">Poor Man's System</a>
            <button class="navbar-toggler pull-right" type="button" data-bs-toggle="collapse" data-bs-target="#navbarScroll"
                aria-controls="navbarScroll" aria-expanded="false" aria-label="Toggle navigation"
                @click.prevent="toggleNavbar()">
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" :class="navbarActiveClass" id="navbarNav">
                <ul class="navbar-nav me-auto my-2 my-lg-0" style="--bs-scroll-height: 100px;">
                    <li v-for="(page, index) in pages" class="nav-item" :key="index">
                        <navbar-link :page="page" :isActive="activePage === index" :pageIndex="index"
                            @click.prevent="navLinkClick(index)">

                        </navbar-link>
                    </li>
                </ul>
                <button class="btn btn-primary pull-right" @click.preven="toPanelPage()"> {{ this.$root.user === null ? "Login" : "Panel" }} </button>
            </div>
            
        </div>
    </nav>
</template>

<script>
import NavbarLink from './NavbarLink.vue'

export default {
    components: {
        NavbarLink
    },
    computed: {
        navbarActiveClass() {
            return {
                collapse: !this.navbarActive,
                show: this.navbarActive,
            }
        }
    },
    props: ['pages', 'activePage', 'navLinkClick'],
    data() {
        return {
            theme: 'light',
            navbarActive: 0
        }
    },
    methods: {
        changeTheme() {
            this.theme = this.theme == 'light' ? 'dark' : 'light';
        },
        returnHome() {
            this.$root.activePage = this.$root.appPage.ROOT;
        },
        toggleNavbar() {
            this.navbarActive ^= 1;
        },
        toPanelPage() {
            this.$root.activePage = this.$root.appPage.PANEL;
        }
    }
}
</script>